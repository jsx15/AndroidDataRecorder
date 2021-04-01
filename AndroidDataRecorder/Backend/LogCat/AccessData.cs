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
        /// Object to restrain the access on a file to only one Thread at a time
        /// </summary>
        private Object _fileInUse = new object();

        /// <summary>
        /// The needed values to calculate the workload variables
        /// </summary>
        private String _memTotal, _memAvailiable;

        /// <summary>
        /// The workload variables
        /// </summary>
        private int _cpuUsage, _memUsed, _batteryLevel;

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
        /// Creates a logcat process and calls saveLogs
        /// Starts a new Thread to log the workload data of the device
        /// </summary>
        /// <param name="device"> The device </param>
        /// <param name="client"> The AdbClient </param>
        /// <param name="receiver"> The ConsoleOutputReceiver </param>
        public void InitializeProcess(DeviceData device, AdbClient client, ConsoleOutputReceiver receiver)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo 
                {
                    FileName = Config.getAdbPath(), 
                    Arguments = "-s " + device.Serial + " logcat -v year", 
                    UseShellExecute = false, 
                    RedirectStandardOutput = true, 
                    CreateNoWindow = true
                }
            };
            
            new Thread(() => AccessWorkload(device, client, receiver)).Start();
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
                    foreach (var item in grokResult)
                    {
                        //Console.WriteLine($"{item.Key} : {item.Value}");
                    }

                    /*lock (fileInUse)
                    {
                        using (StreamWriter w = File.AppendText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\LogDaten.log"))
                        {
                            w.WriteLine(line);
                        }
                        Console.WriteLine(line);
                    }*/
                }
            }
        }

        /// <summary>
        /// Access the CpuUsage, MemoryUsage and the BatteryLevel periodically every 30 seconds
        /// </summary>
        /// <param name="device"> The device to be checked </param>
        /// <param name="receiver"> The needed receiver </param>
        /// <param name="client"> The AdbClient </param>
        private void AccessWorkload(DeviceData device, AdbClient client, ConsoleOutputReceiver receiver)
        {
            try
            {
                while (device.State == DeviceState.Online)
                {
                    client.ExecuteRemoteCommand("dumpsys cpuinfo", device, receiver);
                    var m = Regex.Match(receiver.ToString(), @"(\S+)\s+TOTAL");
                    if (m.Success) 
                    {
                        _cpuUsage = Convert.ToInt32(float.Parse(m.Groups[1].Value.Remove(m.Groups[1].Value.Length - 1, 1)));
                    }
            
                    client.ExecuteRemoteCommand("cat /proc/meminfo", device, receiver);
                    m = Regex.Match(receiver.ToString(), @"MemTotal: +(\S+)\s");
                    if (m.Success) 
                    {
                        _memTotal = m.Groups[1].Value;
                    }
            
                    m = Regex.Match(receiver.ToString(), @"MemAvailable: +(\S+)\s");
                    if (m.Success) 
                    {
                        _memAvailiable = m.Groups[1].Value;
                    }
            
                    _memUsed = Convert.ToInt32((float.Parse(_memAvailiable)/float.Parse(_memTotal))*100);
            
                    client.ExecuteRemoteCommand("dumpsys battery", device, receiver);
                    m = Regex.Match(receiver.ToString(), @"level: +(\S+)\s");
                    if (m.Success)
                    {
                        _batteryLevel = Int32.Parse(m.Groups[1].Value);
                    }
                
                    _database.InsertValuesInTableResources(device.Name, _cpuUsage, _memUsed, _batteryLevel, DateTime.Now);
            
                    Thread.Sleep(30000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}