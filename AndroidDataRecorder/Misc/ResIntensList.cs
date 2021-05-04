using System;

namespace AndroidDataRecorder.Misc
{
    public class ResIntensList
    {
        public string Serial { get; set; }
        public string DeviceName { get; set; }
        public double Cpu { get; set; }
        public double Memory { get; set; }
        public string Process { get; set; }

        public DateTime Timestamp { get; set; }
        
        public ResIntensList(){}

        public ResIntensList(string serial, string deviceName, double cpu, double memory, string process, DateTime timestamp)
        {
            Serial = serial;
            DeviceName = deviceName;
            Cpu = cpu;
            Memory = memory;
            Process = process;
            Timestamp = timestamp;
        }
    }
}