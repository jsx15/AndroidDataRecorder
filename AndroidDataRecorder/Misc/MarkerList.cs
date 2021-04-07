using System.Collections.Generic;

namespace AndroidDataRecorder.Misc
{
    public class MarkerList
    {
        /*
         * List for all markers
         */
        public static List<Marker> Markers = new List<Marker>();

        /*
         * Selected device
         */
        public static SharpAdbClient.DeviceData ActiveDeviceData;

        /*
         * Database
         */
        private Database.Database data = new Database.Database();

        /*
         * Constructor
         */
        public MarkerList()
        {
            if (ActiveDeviceData != null)
            {
                Markers = data.ListWithMarker(ActiveDeviceData.Name);
            }
        }

        /*
         * Update marker list
         */
        public void Update()
        {
            Markers = data.ListWithMarker(ActiveDeviceData.Name);
        }

        /*
         * Change device
         */
        public void SetDevice(SharpAdbClient.DeviceData device)
        {
            ActiveDeviceData = device;
            Update();
        }
        
        /*
         * Active device name
         */
        public string GetDeviceName()
        {
            return ActiveDeviceData is not null ? ActiveDeviceData.Name : "";
        }
    
    }
}