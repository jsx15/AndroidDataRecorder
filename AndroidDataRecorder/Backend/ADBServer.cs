using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using AndroidDataRecorder.Backend.LogCat;
using AndroidDataRecorder.Pages;
using SharpAdbClient;

namespace AndroidDataRecorder.Backend
{
    public static class AdbServer
    {
        /// <summary>
        /// The AdbServer
        /// </summary>
        private static readonly SharpAdbClient.AdbServer Server = new SharpAdbClient.AdbServer();
        
        /// <summary>
        /// The AdbClient
        /// </summary>
        private static readonly AdbClient Client = new AdbClient();
        
        /// <summary>
        /// The Receiver
        /// </summary>
        private static readonly ConsoleOutputReceiver Receiver = new ConsoleOutputReceiver();

        /// <summary>
        /// Object of AccessData
        /// </summary>
        private static AccessData _accessData;

        /// <summary>
        /// Initialize Adb Server and Monitor
        /// </summary>
        public static void InitializeAdbServer(string path)
        {
            var result = Server.StartServer(Path.GetFullPath(Path.Combine(path)), restartServerIfNewer: false);
            var monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            monitor.DeviceConnected += OnDeviceConnected;
            monitor.DeviceConnected += Configuration.ConnectedDeviceChanged;
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
                Client.Connect(ipAddressDevice);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the connected devices
        /// </summary>
        /// <returns> List of the devices </returns>
        public static List<DeviceData> GetConnectedDevices()
        {
            var deviceList = new List<DeviceData>();
            foreach (var device in Client.GetDevices())
            {
                if (device.State != DeviceState.NoPermissions)
                {
                    deviceList.Add(device);
                }
            }

            return deviceList;
        }

        /// <summary>
        /// Restart the AdbServer
        /// </summary>
        /// <returns> true for success and false for failure </returns>
        public static bool RestartAdbServer()
        {
            try
            {
                Server.RestartServer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Kill the AdbServer
        /// </summary>
        /// <returns> true for success and false for failure </returns>
        public static bool KillAdbServer()
        {
            try
            {
                Client.KillAdb();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Start logging when a new device connects and display a toast
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event to recognize devices </param>
        private static void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            _accessData = new AccessData();
            try
            {
                foreach (var device in GetConnectedDevices())
                {
                    if (device.Serial.Equals(e.Device.Serial))
                    {
                        Client.ExecuteRemoteCommand("logcat -b all -c", e.Device, Receiver);
                        new Thread(() => _accessData.InitializeProcess(device, Client, Receiver)).Start();
                    }
                }
                Console.WriteLine($"The device {e.Device} has connected to this PC");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Display a toast
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event to recognize devices </param>
        private static void OnDeviceDisconnected(object sender, DeviceDataEventArgs e)
        {
            Console.WriteLine($"The device {e.Device} has disconnected from this PC");
        }

        /// <summary>
        ///  Get adb version running on host
        /// </summary>
        /// <returns>ADB Version running</returns>
        public static int GetAdbVersion()
        {
            return Client.GetAdbVersion();
        }

        public static bool ServerStatus()
        {
             return Server.GetStatus().IsRunning;
        }
    }
}