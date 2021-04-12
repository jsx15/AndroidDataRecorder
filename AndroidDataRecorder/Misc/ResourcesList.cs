using System;

namespace AndroidDataRecorder.Misc
{
    public class ResourcesList
    {
        public string deviceName { get; set; }
        public int cpu { get; set; }
        public int memory { get; set; }
        public int battery { get; set; }
        public DateTime timestamp { get; set; }
        
        public ResourcesList(){}

        public ResourcesList(string _deviceName, int _cpu, int _memory, int _battery, DateTime _timestamp)
        {
            deviceName = _deviceName;
            cpu = _cpu;
            memory = _memory;
            battery = _battery;
            timestamp = _timestamp;
        }
    }
}