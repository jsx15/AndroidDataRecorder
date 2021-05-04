using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AndroidDataRecorder.Database;
using GrokNet;
using SharpAdbClient;

namespace AndroidDataRecorder.Backend
{
    public class AccessData
    {
        /// <summary>
        /// The needed values to calculate the workload variables
        /// </summary>
        private double _memTotal, _memAvailable, _cpuTotal, _cpuIdle;

        /// <summary>
        /// The workload variables
        /// </summary>
        private int _batteryLevel;

        /// <summary>
        /// The shell command to get the cpu usage
        /// It is individually adapted to to Android version
        /// </summary>
        private string _cpuUsageCommand;

        /// <summary>
        /// The database to write into
        /// </summary>
        private readonly TableDevices _tableDevices = new TableDevices();
        private readonly TableApp _tableApp = new TableApp();
        private readonly TableLogs _tableLogs = new TableLogs();
        private readonly TableResources _tableResources = new TableResources();
        private readonly TableResIntens _tableResIntens = new TableResIntens();
        
        /// <summary>
        /// The grok to filter the logs
        /// </summary>
        private readonly Grok _grok = new Grok(
            "%{DATA:serial} %{USERNAME:device} %{TIMESTAMP_ISO8601:system_timestamp} %{TIMESTAMP_ISO8601:device_timestamp}%{SPACE}%{NUMBER:PID}%{SPACE}%{NUMBER:TID}%{SPACE}%{WORD:loglevel}%{SPACE}%{DATA:App}%{SPACE}:%{SPACE}%{GREEDYDATA:LogMessage}"
        );

        /// <summary>
        /// The device that should be logged
        /// </summary>
        private DeviceData _device;

        /// <summary>
        /// The CancellationToken for the Thread
        /// </summary>
        private CancellationToken _token;
        
        /// <summary>
        /// Initialize by setting the device
        /// </summary>
        /// <param name="device"> The device that should be logged </param>
        public AccessData(DeviceData device)
        {
            _device = device;
        }

        /// <summary>
        /// Starts a Stopwatch and gives the device 30 seconds to change its state to online
        /// If the device state changes to online it starts logging
        /// If the device state doesn't change to online it will just execute
        /// </summary>
        /// <param name="obj"> the CancellationToken </param>
        public void CheckDeviceState(object obj)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            _token = (CancellationToken) obj;

            do
            {
                try
                {
                    if (_device.State == DeviceState.Online)
                    {
                        foreach (var d in AdbServer.GetConnectedDevices())
                        {
                            if (d.Serial.Equals(_device.Serial))
                            {
                                watch.Stop();
                                _device = d;
                                if(!_tableDevices.DeviceList().Exists(x => x.Serial.Equals(_device.Serial))) _tableDevices.InsertValues(_device.Serial, _device.Name);
                                new Thread(SaveApps).Start();
                                InitializeProcess();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            while (watch.Elapsed.Milliseconds < 30000 && watch.IsRunning);
        }
        
        /// <summary>
        /// Creates a logcat process and calls saveLogs
        /// Starts a new Thread to log the workload data of the device
        /// </summary>
        private void InitializeProcess()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo 
                {
                    FileName = Config.GetAdbPath(), 
                    Arguments = "-s " + _device.Serial + " logcat -v year", 
                    UseShellExecute = false, 
                    RedirectStandardOutput = true, 
                    CreateNoWindow = true
                }
            };

            try
            {
                var receiver = new ConsoleOutputReceiver();

                //Empty the logs before starting to log
                AdbServer.GetClient().ExecuteRemoteCommand("logcat -b all -c", _device, receiver);
            
                receiver = new ConsoleOutputReceiver();
            
                //Decide which command to use for accessing the cpu usage by checking the devices build version
                AdbServer.GetClient().ExecuteRemoteCommand("getprop ro.build.version.release", _device, receiver);
                _cpuUsageCommand = Convert.ToInt32(receiver.ToString()) < 9 ? "top  1" : "top -b -m 5 -n 1";
            
                //Start the logging and accessing of the workload
                new Thread(() => SaveLogs(proc)).Start();
                AccessWorkload();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Save the third party apps into the according table
        /// </summary>
        private void SaveApps()
        {
            var receiver = new ConsoleOutputReceiver();
            
            _tableApp.DeleteRow(_device.Serial);
            AdbServer.GetClient().ExecuteRemoteCommand("pm list packages -3", _device, receiver);
            var appList = receiver.ToString().Split("\r\n").ToList();
            for (var i = 0; i < appList.Count - 1; i++)
            {
                _tableApp.InsertValues(appList[i], _device.Serial);
            }
        }
        
        /// <summary>
        /// Write all the logs consistently into the database table Logs
        /// </summary>
        /// <param name="proc"> The process to be executed </param>
        private void SaveLogs(Process proc)
        {
            proc.Start();
            while (!proc.StandardOutput.EndOfStream && !_token.IsCancellationRequested) {
                string line = proc.StandardOutput.ReadLine();
                if (!string.IsNullOrEmpty(line) && !line.StartsWith("---------"))
                {
                    line = _device.Serial + " " + _device.Name + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + line;
                    
                    var grokResult = _grok.Parse(line);
                    _tableLogs.InsertValues(grokResult[0].Value.ToString(), grokResult[1].Value.ToString(), 
                        Convert.ToDateTime(grokResult[2].Value), Convert.ToDateTime(grokResult[3].Value), 
                        Convert.ToInt32(grokResult[4].Value), Convert.ToInt32(grokResult[5].Value), 
                        grokResult[6].Value.ToString(), grokResult[7].Value.ToString(), grokResult[8].Value.ToString());
                }
            }
        }

        /// <summary>
        /// Get the CpuUsage, MemoryUsage and the BatteryLevel periodically every 30 seconds and write them into the database --> Resources
        /// Also gets the five most expensive processes and their CPU/mem usage and writes them into the database --> ResIntens
        /// </summary>
        private void AccessWorkload()
        {
            while(!_token.IsCancellationRequested && AdbServer.GetConnectedDevices().Exists(x => x.Serial.Equals(_device.Serial)))
            {
                var receiver = new ConsoleOutputReceiver();

                try
                {
                    //Get the cpu usage and the five most expensive processes
                    AdbServer.GetClient().ExecuteRemoteCommand(_cpuUsageCommand, _device, receiver);
                    var cpu = GetCpuUsage(receiver.ToString());
                    var fiveProcesses = GetFiveProcesses(receiver.ToString());
                    var cpuFiveProcesses = GetCpuFiveProcesses(receiver.ToString());
                    var memFiveProcesses = GetMemFiveProcesses(receiver.ToString());
                    
                    //Get the memusage
                    AdbServer.GetClient().ExecuteRemoteCommand("cat /proc/meminfo", _device, receiver);
                    var mem = GetMemUsage(receiver.ToString());
            
                    //Get the battery level
                    AdbServer.GetClient().ExecuteRemoteCommand("dumpsys battery", _device, receiver);
                    GetBatteryLevel(receiver.ToString());

                    var time = DateTime.Now;
                    
                    //Insert CPU/mem usage and the battery level into Resources
                    _tableResources.InsertValues(_device.Serial, _device.Name, cpu, mem, _batteryLevel, time);

                    //Insert the five most expensive processes into ResIntens
                    for (var i = 0; i < 5; i++)
                    {
                        _tableResIntens.InsertValues(_device.Serial, _device.Name,
                            Math.Round((double.Parse(cpuFiveProcesses[i].ToString(), CultureInfo.InvariantCulture) / _cpuTotal) * 100, 2),
                            double.Parse(memFiveProcesses[i].ToString(), CultureInfo.InvariantCulture),
                            fiveProcesses[i].ToString(), time);
                    }

                    //Invoke the DeviceWorkloadChanged event and wait 30 seconds
                    AdbServer.CustomMonitor.Instance.OnDeviceWorkloadChanged(new DeviceDataEventArgs(_device));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
                Thread.Sleep(Config.GetAccessWorkloadInterval());
            }
        }

        /// <summary>
        /// Calculate and the cpu usage 
        /// </summary>
        /// <param name="queryString"> The receiver output </param>
        /// <returns> the cpu usage </returns>
        private int GetCpuUsage(string queryString)
        {
            var m = Regex.Match(queryString, @"\w*(?=(\%idle))");
            if (m.Success)
            {
                _cpuIdle = double.Parse(m.Value);
            }
            m = Regex.Match(queryString, @"\w*(?=(\%cpu))");
            if (m.Success)
            {
                _cpuTotal = double.Parse(m.Value);
            }
            
            return Convert.ToInt32(((_cpuTotal - _cpuIdle)/ _cpuTotal)*100);
        }

        /// <summary>
        /// Calculate the memory usage
        /// </summary>
        /// <param name="queryString"> The receiver output </param>
        /// <returns> the memory usage </returns>
        private int GetMemUsage(string queryString)
        {
            var m = Regex.Match(queryString, @"(?<=MemTotal:\s+)([0-9]+)");
            if (m.Success)
            {
                _memTotal = double.Parse(m.Value);
            }
            
            m = Regex.Match(queryString, @"(?<=MemAvailable:\s+)([0-9]+)");
            if (m.Success)
            {
                _memAvailable = double.Parse(m.Value);
            }
            
            return Convert.ToInt32(((_memTotal - _memAvailable)/ _memTotal)*100);
        }

        /// <summary>
        /// get the battery level
        /// </summary>
        /// <param name="queryString"> The receiver output </param>
        private void GetBatteryLevel(string queryString)
        {
            var m = Regex.Match(queryString, @"(?<=level:\s+)([0-9]+)");
            if (m.Success)
            {
                _batteryLevel = int.Parse(m.Groups[1].Value);
            }
        }

        /// <summary>
        /// Get the five most expensive processes
        /// </summary>
        /// <param name="queryString"> The receiver output </param>
        /// <returns> A MatchCollection with those processes </returns>
        private MatchCollection GetFiveProcesses(string queryString)
        {
            var n = Regex.Matches(queryString, @"((?<=[0-9]+\:[0-9]+\.[0-9]+\s)(.*)(?=\s))");
            return n.Count != 0 ? n : null;
        }
        
        /// <summary>
        /// Get the cpu usage of the five most expensive processes
        /// </summary>
        /// <param name="queryString"> The receiver output </param>
        /// <returns> A MatchCollection with those cpu usages </returns>
        private MatchCollection GetCpuFiveProcesses(string queryString)
        {
            var n = Regex.Matches(queryString, @"((?<=[A-Z]\s+)([0-9]+\.?[0-9]*)(?=\s+[0-9]+\.*[0-9]*\s+))");
            return n.Count != 0 ? n : null;
        }
        
        /// <summary>
        /// Get the mem usage of the five most expensive processes
        /// </summary>
        /// <param name="queryString"> The receiver output </param>
        /// <returns> A MatchCollection with those mem usages </returns>
        private MatchCollection GetMemFiveProcesses(string queryString)
        {
            var n = Regex.Matches(queryString, @"((?<=[0-9]+\.?[0-9]*\s+)([0-9]+\.?[0-9]*)(?=\s+[0-9]+\:[0-9]+\.[0-9]+))");
            return n.Count != 0 ? n : null;
        }
    }
}