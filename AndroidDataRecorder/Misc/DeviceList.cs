namespace AndroidDataRecorder.Misc
{
    public class DeviceList
    {
        public string serial { get; set; }
        public string deviceName { get; set; }
        
        public DeviceList(){}

        public DeviceList(string _serial, string _deviceName)
        {
            serial = _serial;
            deviceName = _deviceName;
        }
    }
}