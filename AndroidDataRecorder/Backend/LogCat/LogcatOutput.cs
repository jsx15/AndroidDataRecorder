using System;
using System.Diagnostics;
using System.IO;

namespace AndroidDataRecorder.Backend.LogCat
{
    public static class LogcatOutput
    {
        /// <summary>
        /// Creates a logcat process and calls saveLogs
        /// </summary>
        /// <param name="serialNumber"> The serial number of the device </param>
        /// <param name="deviceName"> The name of the device </param>
        public static void startLogcat(String serialNumber, String deviceName)
        {
            Process proc = null;
            
            if (OperatingSystem.IsLinux())
            {
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/usr/bin/adb",
                        Arguments = "-s " + serialNumber + " logcat",
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
                        Arguments = "-s " + serialNumber + " logcat",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
            }
            
            saveLogs(proc, deviceName);
        }

        /// <summary>
        /// Save the logs to LogDaten.log
        /// </summary>
        /// <param name="proc"> The process to be executed </param>
        /// <param name="deviceName"> The name of the device </param>
        private static void saveLogs(Process proc, String deviceName)
        {
            proc.Start();
            while (!proc.StandardOutput.EndOfStream) {
                string line = deviceName + " " + proc.StandardOutput.ReadLine();
                using (StreamWriter w = File.AppendText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\LogDaten.log"))
                {
                    w.WriteLine(line);
                }
                Console.WriteLine(line);
            }
        }
    }
}