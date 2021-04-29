using System;

namespace AndroidDataRecorder.Misc
{
    public class ResIntensList
    {
        public string serial { get; set; }
        public string deviceName { get; set; }
        public double cpu { get; set; }
        public double memory { get; set; }
        public string process { get; set; }

        public DateTime timestamp { get; set; }
        
        public ResIntensList(){}

        public ResIntensList(string _serial, string _deviceName, double _cpu, double _memory, string _process, DateTime _timestamp)
        {
            serial = _serial;
            deviceName = _deviceName;
            cpu = _cpu;
            memory = _memory;
            process = _process;
            timestamp = _timestamp;
        }
    }
}