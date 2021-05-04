using System;

namespace AndroidDataRecorder.Misc
{
    public class ResourcesList
    {
        /// <summary>
        /// The serial number of the device
        /// </summary>
        public string Serial { get; set; }
        
        /// <summary>
        /// The name of the device
        /// </summary>
        public string DeviceName { get; set; }
        
        /// <summary>
        /// The cpu usage of the device
        /// </summary>
        public int Cpu { get; set; }
        
        /// <summary>
        /// The memory usage of the device
        /// </summary>
        public int Memory { get; set; }
        
        /// <summary>
        /// The battery level of the device
        /// </summary>
        public int Battery { get; set; }
        
        /// <summary>
        /// The timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Empty constructor
        /// </summary>
        public ResourcesList(){}

        /// <summary>
        /// Constructor to set the variables
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="deviceName"></param>
        /// <param name="cpu"></param>
        /// <param name="memory"></param>
        /// <param name="battery"></param>
        /// <param name="timestamp"></param>
        public ResourcesList(string serial, string deviceName, int cpu, int memory, int battery, DateTime timestamp)
        {
            Serial = serial;
            DeviceName = deviceName;
            Cpu = cpu;
            Memory = memory;
            Battery = battery;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Override the ToString method to display all the variables
        /// </summary>
        /// <returns> the variables as string </returns>
        public override string ToString()
        {
            return Timestamp + "Device serial: " + Serial + " Device name: " + DeviceName + " CPU load: " +
                   Cpu + " Memory usage: " + Memory + " Battery status: " + Battery;
        }
    }
}