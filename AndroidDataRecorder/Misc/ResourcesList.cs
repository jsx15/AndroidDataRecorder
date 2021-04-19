using System;

namespace AndroidDataRecorder.Misc
{
    public class ResourcesList
    {
        public string serial { get; set; }
        public string deviceName { get; set; }
        public int cpu { get; set; }
        public int memory { get; set; }
        public int battery { get; set; }
        public DateTime timestamp { get; set; }
        
        public ResourcesList(){}

        public ResourcesList(string serial, string deviceName, int cpu, int memory, int battery, DateTime timestamp)
        {
            this.serial = serial;
            this.deviceName = deviceName;
            this.cpu = cpu;
            this.memory = memory;
            this.battery = battery;
            this.timestamp = timestamp;
        }

        public override string ToString()
        {
            return "##" + timestamp + " Device serial: " + serial + " Device name: " + deviceName + " CPU load: " +
                   cpu + " Memory usage: " + memory + " Battery status: " + battery;
        }
    }
}