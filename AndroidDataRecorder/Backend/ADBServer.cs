using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using AndroidDataRecorder.Backend.LogCat;
using SharpAdbClient;

namespace AndroidDataRecorder.Backend
{
    public static class ADBServer
    {
        /// <summary>
        /// The AdbServer
        /// </summary>
        private static readonly AdbServer _server = new AdbServer();
        
        /// <summary>
        /// The AdbClient
        /// </summary>
        private static readonly AdbClient _client = new AdbClient();

        /// <summary>
        /// Initialize Adb Server and Monitor
        /// </summary>
        static ADBServer()
        {
            if (OperatingSystem.IsLinux())
            {
                var result = _server.StartServer(Path.GetFullPath(Path.Combine("/usr/bin/adb")), restartServerIfNewer: false);
            }
            else if (OperatingSystem.IsWindows())
            {
                var result = _server.StartServer(Path.GetFullPath(Path.Combine(@"C:\Program Files (x86)\platform-tools\adb.exe")), restartServerIfNewer: false);
            }
            
            var monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            monitor.DeviceConnected += OnDeviceConnected;
            monitor.DeviceDisconnected += OnDeviceDisconnected;
            monitor.Start();
        }

        /// <summary>
        /// Connect to a Android device over a Network
        /// </summary>
        /// <param name="ipAddressDevice"> The IP address of the Android device </param>
        /// <returns> true for a succeed and false for failure </returns>
        public static bool ConnectWirelessCLient(String ipAddressDevice)
        {
            try
            {
                _client.Connect(ipAddressDevice);
            }
            catch (System.InvalidOperationException e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the connected devices
        /// </summary>
        /// <returns> List of the devices for succeed and null for failure </returns>
        public static List<DeviceData> GetConnectedDevices()
        {
            try
            {
                return _client.GetDevices();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        /// <summary>
        /// Start logging when a new device connects and display a toast
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event to reccognize devices </param>
        static void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            new Thread(() => LogcatOutput.startLogcat(e.Device.Serial, e.Device.Name)).Start();
            Console.WriteLine($"The device {e.Device.Name} has connected to this PC");
        }

        /// <summary>
        /// Display a toast
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event to reccognize devices </param>
        static void OnDeviceDisconnected(object sender, DeviceDataEventArgs e)
        {
            Console.WriteLine($"The device {e.Device.Name} has disconnected from this PC");
        }
    }
}