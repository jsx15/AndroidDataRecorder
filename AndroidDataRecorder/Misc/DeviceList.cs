namespace AndroidDataRecorder.Misc
{
    public class DeviceList
    {
        public string Serial { get; set; }
        public string DeviceName { get; set; }
        
        public DeviceList(){}

        public DeviceList(string serial, string deviceName)
        {
            Serial = serial;
            DeviceName = deviceName;
        }
    }
}