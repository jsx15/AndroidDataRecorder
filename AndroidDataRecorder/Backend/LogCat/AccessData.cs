using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using GrokNet;
using SharpAdbClient;

namespace AndroidDataRecorder.Backend.LogCat
{
    public class AccessData
    {
        /// <summary>
        /// A delegate to specify the EventHandler type
        /// </summary>
        public delegate void DeviceOnlineEvent();

        /// <summary>
        /// A event to indicate that a connected device is now online
        /// </summary>
        public event DeviceOnlineEvent DeviceIsOnline;
        
        /// <summary>
        /// The needed values to calculate the workload variables
        /// </summary>
        private double _memTotal, _memAvailable, _cpuTotal, _cpuIdle;

        /// <summary>
        /// The workload variables
        /// </summary>
        private int _batteryLevel;

        /// <summary>
        /// The database to write into
        /// </summary>
        private Database.Database _database = new Database.Database();
        
        /// <summary>
        /// The grok to filter the logs
        /// </summary>
        private Grok grok = new Grok(
            "%{USERNAME:device} %{TIMESTAMP_ISO8601:system_timestamp} %{TIMESTAMP_ISO8601:device_timestamp}%{SPACE}%{NUMBER:PID}%{SPACE}%{NUMBER:TID}%{SPACE}%{WORD:loglevel}%{SPACE}%{DATA:App}%{SPACE}:%{SPACE}%{GREEDYDATA:LogMessage}"
        );

        /// <summary>
        /// Starts a Stopwatch and gives the device 30 seconds to change its state to online
        /// If the device state changes to online it starts logging
        /// If the device state doesn't change to online it will just execute
        /// </summary>
        /// <param name="device"> The device </param>
        /// <param name="client"> The AdbClient </param>
        public void CheckDeviceState(DeviceData device, AdbClient client)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            do
            {
                try
                {
                    if (device.State == DeviceState.Online)
                    {
                        foreach (var d in AdbServer.GetConnectedDevices())
                        {
                            if (d.Serial.Equals(device.Serial))
                            {
                                watch.Stop();
                                DeviceIsOnline?.Invoke();
                                InitializeProcess(d, client);
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
        /// <param name="device"> The device </param>
        /// <param name="client"> The AdbClient </param>
        public void InitializeProcess(DeviceData device, AdbClient client)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo 
                {
                    FileName = Config.GetAdbPath(), 
                    Arguments = "-s " + device.Serial + " logcat -v year", 
                    UseShellExecute = false, 
                    RedirectStandardOutput = true, 
                    CreateNoWindow = true
                }
            };
            
            var receiver = new ConsoleOutputReceiver();
            new Thread(() => AccessWorkload(device, client)).Start();
            client.ExecuteRemoteCommand("logcat -b all -c", device, receiver);
            SaveLogs(proc, device.Name);
        }

        /// <summary>
        /// Save the logs to LogDaten.log located in home/user
        /// </summary>
        /// <param name="proc"> The process to be executed </param>
        /// <param name="deviceName"> The name of the device </param>
        private void SaveLogs(Process proc, String deviceName)
        {
            proc.Start();
            while (!proc.StandardOutput.EndOfStream) {
                string line = proc.StandardOutput.ReadLine();
                if (!string.IsNullOrEmpty(line) && !line.StartsWith("---------"))
                {
                    line = deviceName + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + line;
                    
                    var grokResult = grok.Parse(line);
                    _database.InsertValuesInTableLogs(grokResult[0].Value.ToString(), 
                        Convert.ToDateTime(grokResult[1].Value), Convert.ToDateTime(grokResult[2].Value), 
                        Convert.ToInt32(grokResult[3].Value), Convert.ToInt32(grokResult[4].Value), 
                        grokResult[5].Value.ToString(), grokResult[6].Value.ToString(), grokResult[7].Value.ToString());
                }
            }
        }

        /// <summary>
        /// Get the CpuUsage, MemoryUsage and the BatteryLevel periodically every 30 seconds and write them into the database
        /// </summary>
        /// <param name="device"> The device to be checked </param>
        /// <param name="client"> The AdbClient </param>
        private void AccessWorkload(DeviceData device, AdbClient client)
        {
            try
            {
                while (device.State == DeviceState.Online)
                {
                    var receiver = new ConsoleOutputReceiver();
                    
                    client.ExecuteRemoteCommand("top -b -m 5 -n 1", device, receiver);

                    var cpu = GetCpuUsage(receiver.ToString());
                    var fiveProcesses = GetFiveProcesses(receiver.ToString());
                    var cpuFiveProcesses = GetCpuFiveProcesses(receiver.ToString());
                    var memFiveProcesses = GetMemFiveProcesses(receiver.ToString());
                    
                    client.ExecuteRemoteCommand("cat /proc/meminfo", device, receiver);

                    var mem = GetMemUsage(receiver.ToString());
            
                    client.ExecuteRemoteCommand("dumpsys battery", device, receiver);

                    GetBatteryLevel(receiver.ToString());
                    
                    _database.InsertValuesInTableResources(device.Name, cpu, mem, _batteryLevel, DateTime.Now);
            
                    Thread.Sleep(30000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                _batteryLevel = Int32.Parse(m.Groups[1].Value);
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
            if (n.Count != 0)
            {
                return n;
            }

            return null;
        }
        
        /// <summary>
        /// Get the cpu usage of the five most expensive processes
        /// </summary>
        /// <param name="queryString"> The receiver output </param>
        /// <returns> A MatchCollection with those cpu usages </returns>
        private MatchCollection GetCpuFiveProcesses(string queryString)
        {
            var n = Regex.Matches(queryString, @"((?<=[A-Z]\s+)([0-9]+\.[0-9])(?=\s+[0-9]+\.[0-9]\s+))");
            if (n.Count != 0)
            {
                return n;
            }

            return null;
        }
        
        /// <summary>
        /// Get the mem usage of the five most expensive processes
        /// </summary>
        /// <param name="queryString"> The receiver output </param>
        /// <returns> A MatchCollection with those mem usages </returns>
        private MatchCollection GetMemFiveProcesses(string queryString)
        {
            var n = Regex.Matches(queryString, @"((?<=[0-9]+\.[0-9]\s+)([0-9]+\.[0-9])(?=\s+[0-9]+\:[0-9]+\.[0-9]+))");
            if (n.Count != 0)
            {
                return n;
            }

            return null;
        }
    }
}