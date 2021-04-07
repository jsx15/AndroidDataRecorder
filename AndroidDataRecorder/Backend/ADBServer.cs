using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private static readonly AccessData AccessData = new AccessData();

        /// <summary>
        /// Initialize Adb Server and Monitor
        /// </summary>
        public static void InitializeAdbServer(string path)
        {
            var result = Server.StartServer(Path.GetFullPath(Path.Combine(path)), restartServerIfNewer: false);
            var monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            monitor.DeviceConnected += OnDeviceConnected;
            monitor.DeviceDisconnected += OnDeviceDisconnected;
            // monitor.DeviceConnected += Configuration.ConnectedDeviceChanged;
            monitor.Start();
        }

        /// <summary>
        /// Connect to a Android device over a Network
        /// </summary>
        /// <param name="ipAddressDevice"> The IP address of the Android device </param>
        /// <returns> true for a succeed and false for failure </returns>
        public static bool ConnectWirelessClient(String ipAddressDevice)
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


        public static void DisconnectWirelessClient(String ipAddressDevice)
        {
            try
            {
                Client.Disconnect(new DnsEndPoint(ipAddressDevice, 5555));
            }
            catch (Exception)
            {
            }
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
            try
            {
                foreach (var device in GetConnectedDevices())
                {
                    Thread.Sleep(250);
                    if (device.State != DeviceState.Online)
                    {
                        break;
                    }


                    if (device.Serial.Equals(e.Device.Serial))
                    {
                        Client.ExecuteRemoteCommand("logcat -b all -c", e.Device, Receiver);
                        new Thread(() => AccessData.InitializeProcess(device, Client, Receiver)).Start();
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