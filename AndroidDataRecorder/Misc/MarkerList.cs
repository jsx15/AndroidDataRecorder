using System.Collections.Generic;
using System.Linq;
using SharpAdbClient;

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
        private static SharpAdbClient.DeviceData selectedDeviceData = new DeviceData();

        /*
         * Database
         */
        private Database.Database data = new Database.Database();

        /*
         * Constructor
         */
        public MarkerList()
        {
            if (selectedDeviceData != null)
            {
                Markers = data.ListWithMarker("whyred");
                // Markers = data.ListWithMarker(selectedDeviceData.Name);
            }
        }

        /*
         * Update marker list
         */
        public void Update()
        {
            Markers = data.ListWithMarker("whyred");
            // Markers = data.ListWithMarker(selectedDeviceData.Name);
        }

        /*
         * Change device
         */
        public void SetDevice(SharpAdbClient.DeviceData device)
        {
            selectedDeviceData = device;
            Update();
        }
        
        /*
         * Get selectedDevice name
         */
        public string DeviceName()
        {
            return selectedDeviceData.Name;
        }
    }
}