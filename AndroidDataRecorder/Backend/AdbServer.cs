using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
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
        /// A Singleton instance of the CustomMonitor that gets initialized in the constructor of AdbServer
        /// </summary>
        public static CustomMonitor Instance;

        /// <summary>
        /// A custom monitor for customized Events that are based on DeviceDataEvent
        /// It's designed as Singleton
        /// </summary>
        public sealed class CustomMonitor
        {
            private CustomMonitor() {}
            private static readonly object Padlock = new object(); 
            private static CustomMonitor _instance = null;
            public event EventHandler<DeviceDataEventArgs> DeviceWorkloadChanged;
            public event EventHandler<EventArgs> MultipleSameDevices;
            
            /// <summary>
            /// Return a single instance of CustomMonitor if none already exists
            /// </summary>
            public static CustomMonitor Instance
            {
                get
                {
                    lock (Padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CustomMonitor();
                        }  
                        return _instance;  
                    }  
                }  
            }
            
            /// <summary>
            /// Invoke the event for DeviceWorkloadChanged
            /// </summary>
            /// <param name="e"> The DeviceDataEvent that is invoked </param>
            public void OnDeviceWorkloadChanged(DeviceDataEventArgs e)
            {
                DeviceWorkloadChanged?.Invoke((object) this, e);
            }
            
            /// <summary>
            /// Invoke the event for MultipleSameDevices
            /// </summary>
            /// <param name="e"> The Event that is invoked </param>
            public void OnMultipleSameDevicesConnected(EventArgs e)
            {
                MultipleSameDevices?.Invoke((object) this, e);
            }
        }

        /// <summary>
        /// Initialize Adb Server and Monitor
        /// </summary>
        public static void InitializeAdbServer(string path)
        {
            var result = Server.StartServer(Path.GetFullPath(Path.Combine(path)), restartServerIfNewer: false);
            var monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            monitor.DeviceConnected += OnDeviceConnected;
            monitor.DeviceChanged += OnDeviceChanged;
            monitor.DeviceDisconnected += OnDeviceDisconnected;
            monitor.Start();
            Instance = CustomMonitor.Instance;
        }

        /// <summary>
        /// Get the AdbClient
        /// </summary>
        /// <returns> the AdbClient </returns>
        public static AdbClient GetClient() => Client;
        
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

        /// <summary>
        /// Disconnect a device that was connected over a wireless connection
        /// </summary>
        /// <param name="ipAddressDevice"> The ip address of the device </param>
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
        /// Initialize the logging process for a device
        /// </summary>
        /// <param name="device"> The device to be logged </param>
        public static void InitializeLogging(DeviceData device)
        {
            var accessData = new AccessData(device);
            var cts = new CancellationTokenSource();
            LoggingManager.AddEntry(device.Serial, cts);
            ThreadPool.QueueUserWorkItem(new WaitCallback(accessData.CheckDeviceState), cts.Token);
        }

        /// <summary>
        /// Stop the logging process for a specified device
        /// </summary>
        /// <param name="device"> The specified device </param>
        /// <returns> true for success and false for failure </returns>
        public static bool StopLogging(DeviceData device)
        {
            return LoggingManager.DeleteEntry(device.Serial);
        }

        /// <summary>
        /// Determines if a device is logging
        /// </summary>
        /// <param name="device"></param>
        /// <returns> true if the device is logging </returns>
        public static bool DeviceIsLogging(DeviceData device)
        {
            return LoggingManager.GetLoggingDevices().ContainsKey(device.Serial);
        }
        
        /// <summary>
        /// Start logging when a new device connects and display a toast
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event to recognize devices </param>
        private static void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            ThreadPool.SetMaxThreads(GetConnectedDevices().Count, GetConnectedDevices().Count);
            InitializeLogging(e.Device);
            Console.WriteLine($"The device {e.Device} has connected to this PC");
        }

        /// <summary>
        /// Check if the device is connected multiple times when it changes it's state
        /// If so, invoke the MultipleSameDevices event
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event to recognize devices </param>
        private static void OnDeviceChanged(object sender, DeviceDataEventArgs e)
        {
            if(Client.GetDevices().FindAll(x => x.Serial.Equals(e.Device.Serial)).Count > 1) Instance.OnMultipleSameDevicesConnected(new EventArgs());
        }
        
        /// <summary>
        /// Display a toast
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event to recognize devices </param>
        private static void OnDeviceDisconnected(object sender, DeviceDataEventArgs e)
        {
            ThreadPool.SetMaxThreads(GetConnectedDevices().Count, GetConnectedDevices().Count);
            LoggingManager.DeleteEntry(e.Device.Serial);
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