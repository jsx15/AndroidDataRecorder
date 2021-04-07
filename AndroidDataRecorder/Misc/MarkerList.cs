using System;
using System.Collections.Generic;
using AndroidDataRecorder.Backend;
using SharpAdbClient;

namespace AndroidDataRecorder.Misc
{
    public class MarkerList
    {
        /// <summary>
        /// List for all markers
        /// </summary>
        public static List<Marker> Markers = new List<Marker>();

        /// <summary>
        /// Selected device
        /// </summary>
        public static SharpAdbClient.DeviceData ActiveDeviceData;

        /// <summary>
        /// Database
        /// </summary>
        private Database.Database data = new Database.Database();

        /// <summary>
        /// Constructor
        /// </summary>
        public MarkerList()
        {
            if (ActiveDeviceData != null)
            {
                Markers = data.ListWithMarker(ActiveDeviceData.Name);
            }
        }

        /// <summary>
        /// Update marker list
        /// </summary>
        public void Update()
        {
            Markers = data.ListWithMarker(ActiveDeviceData.Name);
        }

        /// <summary>
        /// Change device
        /// </summary>
        /// <param name="device">new selected device</param>
        public void SetDevice(SharpAdbClient.DeviceData device)
        {
            ActiveDeviceData = device;
            Update();
        }

        /// <summary>
        /// Active device name
        /// </summary>
        /// <returns>device name</returns>
        public string GetDeviceName()
        {
            return ActiveDeviceData is not null ? ActiveDeviceData.Name : "";
        }

        /// <summary>
        /// Device connection type
        /// </summary>
        /// <returns>Connection of selected device(Wifi or Usb)</returns>
        public string DeviceConnectionType()
        {
            try
            {
                return DeviceStates.ConnectionType(ActiveDeviceData).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Determine the ip address
        /// </summary>
        /// <returns>IP Address of selected device</returns>
        public string IpAddress()
        {
            try
            {
                return DeviceStates.IpAddress(ActiveDeviceData);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string DeviceStatus()
        {
            if (ActiveDeviceData is not null)
            {
                return "connected";
            }

            return "not connected";
        }
    }
}