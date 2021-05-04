using System;

namespace AndroidDataRecorder.Misc
{
    public class ResourcesList
    {
        public string Serial { get; set; }
        public string DeviceName { get; set; }
        public int Cpu { get; set; }
        public int Memory { get; set; }
        public int Battery { get; set; }
        public DateTime Timestamp { get; set; }
        
        public ResourcesList(){}

        public ResourcesList(string serial, string deviceName, int cpu, int memory, int battery, DateTime timestamp)
        {
            this.Serial = serial;
            this.DeviceName = deviceName;
            this.Cpu = cpu;
            this.Memory = memory;
            this.Battery = battery;
            this.Timestamp = timestamp;
        }

        public override string ToString()
        {
            return Timestamp + "Device serial: " + Serial + " Device name: " + DeviceName + " CPU load: " +
                   Cpu + " Memory usage: " + Memory + " Battery status: " + Battery;
        }
    }
}