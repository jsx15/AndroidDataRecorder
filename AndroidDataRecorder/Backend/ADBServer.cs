using System;
using System.Collections.Generic;
using SharpAdbClient;

namespace AndroidDataRecorder.Backend
{
    public static class ADBServer
    {
        /// <summary>
        /// The AdbServer
        /// </summary>
        private static readonly AdbServer _server;
        
        /// <summary>
        /// The AdbClient
        /// </summary>
        private static readonly AdbClient _client;

        /// <summary>
        /// Initialize the Adb Server and Client
        /// </summary>
        static ADBServer()
        {
            _server = new AdbServer();
            var result = _server.StartServer(@"C:\Program Files (x86)\platform-tools\adb.exe", restartServerIfNewer: false);
            _client = new AdbClient();
        }

        /// <summary>
        /// Connect to a Android device over a Network
        /// </summary>
        /// <param name="ipAddressDevice"> The IP address of the Android device </param>
        /// <returns> true for a succeed and false for failure </returns>
        static bool ConnectWirelessCLient(String ipAddressDevice)
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
        static List<DeviceData> GetConnectedDevices()
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
    }
}