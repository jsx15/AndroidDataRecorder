using System;
using System.Collections.Generic;
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
        public static DeviceData ActiveDeviceData;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Database.Database _data = new Database.Database();

        /// <summary>
        /// Constructor
        /// </summary>
        public MarkerList()
        {
            if (ActiveDeviceData != null)
            {
                Markers = _data.ListWithMarker(ActiveDeviceData.Name);
            }
        }

        /// <summary>
        /// Update marker list
        /// </summary>
        public void Update()
        {
            Markers = _data.ListWithMarker(ActiveDeviceData.Name);
        }

        /// <summary>
        /// Change device
        /// </summary>
        /// <param name="device">new selected device</param>
        public void SetDevice(DeviceData device)
        {
            ActiveDeviceData = device;
            Update();
        }

        /// <summary>
        /// Active device name
        /// </summary>
        /// <returns>device name</returns>
        public static string GetDeviceName()
        {
            return ActiveDeviceData is not null ? ActiveDeviceData.Name : "";
        }

        /// <summary>
        /// Device connection type
        /// </summary>
        /// <returns>Connection of selected device(Wifi or Usb)</returns>
        public static string DeviceConnectionType()
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
        public static string IpAddress()
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

        public static string DeviceStatus()
        {
            return ActiveDeviceData is not null ? "connected" : "not connected";
        }
    }
}