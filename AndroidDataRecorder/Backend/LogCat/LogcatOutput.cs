using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using SharpAdbClient;

namespace AndroidDataRecorder.Backend.LogCat
{
    public class AccessData
    {
        /// <summary>
        /// Object to restrain the access on a file to only one Thread at a time
        /// </summary>
        private Object fileInUse = new object();

        /// <summary>
        /// The needed values to calculate the workload variables
        /// </summary>
        private String memTotal, memAvailiable;

        /// <summary>
        /// The workload variables
        /// </summary>
        private int cpuUsage, memUsed, batteryLevel;

        /// <summary>
        /// The database to write into
        /// </summary>
        private Database.Database _database = new Database.Database();

        /// <summary>
        /// Connect to the database
        /// </summary>
        /*public AccessData()
        {
            _database.ConectionToDatabase();
        }*/
        
        /// <summary>
        /// Creates a logcat process and calls saveLogs
        /// Starts a new Thread to log the workload data of the device
        /// </summary>
        /// <param name="serialNumber"> The serial number of the device </param>
        /// <param name="deviceName"> The name of the device </param>
        public void initializeProcess(DeviceData device, AdbClient client, ConsoleOutputReceiver receiver)
        {
            Process proc = null;
            
            if (OperatingSystem.IsLinux())
            {
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/usr/bin/adb",
                        Arguments = "-s " + device.Serial + " logcat -v year",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
            }
            else if (OperatingSystem.IsWindows())
            {
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files (x86)\platform-tools\adb.exe",
                        Arguments = "-s " + device.Serial + " logcat -v year",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
            }
            
            new Thread(() => accessWorkload(device, client, receiver)).Start();
            saveLogs(proc, device.Name);
        }

        /// <summary>
        /// Save the logs to LogDaten.log located in home/user
        /// </summary>
        /// <param name="proc"> The process to be executed </param>
        /// <param name="deviceName"> The name of the device </param>
        private void saveLogs(Process proc, String deviceName)
        {
            proc.Start();
            while (!proc.StandardOutput.EndOfStream) {
                string line = proc.StandardOutput.ReadLine();
                if (line.Length != 0 && !line.StartsWith("---------"))
                {
                    line = deviceName + " " + DateTime.Now + " " + line;
                    lock (fileInUse)
                    {
                        using (StreamWriter w = File.AppendText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\LogDaten.log"))
                        {
                            w.WriteLine(line);
                        }
                        Console.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Access the CpuUsage, MemoryUsage and the BatteryLevel periodically every 30 seconds
        /// </summary>
        /// <param name="device"> The device to be checked </param>
        /// <param name="receiver"> The needed receiver </param>
        /// <param name="client"> The AdbClient </param>
        private void accessWorkload(DeviceData device, AdbClient client, ConsoleOutputReceiver receiver)
        {
            while (device.State == DeviceState.Online)
            {
                client.ExecuteRemoteCommand("dumpsys cpuinfo", device, receiver);
                var m = Regex.Match(receiver.ToString(), @"(\S+)\s+TOTAL");
                if (m.Success) 
                {
                    cpuUsage = Convert.ToInt32(float.Parse(m.Groups[1].Value.Remove(m.Groups[1].Value.Length - 1, 1)));
                }
            
                client.ExecuteRemoteCommand("cat /proc/meminfo", device, receiver);
                m = Regex.Match(receiver.ToString(), @"MemTotal: +(\S+)\s");
                if (m.Success) 
                {
                    memTotal = m.Groups[1].Value;
                }
            
                m = Regex.Match(receiver.ToString(), @"MemAvailable: +(\S+)\s");
                if (m.Success) 
                {
                    memAvailiable = m.Groups[1].Value;
                }
            
                memUsed = Convert.ToInt32((float.Parse(memAvailiable)/float.Parse(memTotal))*100);
            
                client.ExecuteRemoteCommand("dumpsys battery", device, receiver);
                m = Regex.Match(receiver.ToString(), @"level: +(\S+)\s");
                if (m.Success)
                {
                    batteryLevel = Int32.Parse(m.Groups[1].Value);
                }
                
                var name = device.Name;
                Console.WriteLine(device.Name + "          KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
                _database.InsertValuesInTableResources(device.Name, cpuUsage, memUsed, batteryLevel, DateTime.Now);
            
                Thread.Sleep(30000);
            }
        }
    }
}