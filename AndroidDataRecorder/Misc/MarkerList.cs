using System.Collections.Generic;
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
        private readonly Database.Database _data = new Database.Database();

        /// <summary>
        /// Constructor
        /// </summary>
        public MarkerList()
        {
            Markers = new List<Marker>(); 
            if (ActiveDeviceData != null)
            {
                Markers = _data.ListWithMarker(ActiveDeviceData.Serial);
            }
        }

        /// <summary>
        /// Update marker list
        /// </summary>
        public void Update()
        {
            Markers = _data.ListWithMarker(ActiveDeviceData.Serial);
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