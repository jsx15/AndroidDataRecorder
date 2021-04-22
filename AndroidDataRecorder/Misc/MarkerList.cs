using System.Collections.Generic;
using AndroidDataRecorder.Database;
using SharpAdbClient;

namespace AndroidDataRecorder.Misc
{
    public class MarkerList
    {
        /// <summary>
        /// List for all markers
        /// </summary>
        public static List<Marker> Markers { get; set; }

        /// <summary>
        /// Selected device
        /// </summary>
        public static DeviceData ActiveDeviceData;

        /// <summary>
        /// Database
        /// </summary>
        private readonly TableMarker _data = new TableMarker();

        /// <summary>
        /// Constructor
        /// </summary>
        public MarkerList()
        {
            Markers = new List<Marker>(); 
            if (ActiveDeviceData != null)
            {
                Markers = _data.GetList(ActiveDeviceData.Serial);
            }
        }

        /// <summary>
        /// Update marker list
        /// </summary>
        public void Update()
        {
            Markers = _data.GetList(ActiveDeviceData.Serial);
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

        public static string GetDeviceSerial()
        {
            return ActiveDeviceData is not null ? ActiveDeviceData.Serial : "";
        }
    }
}