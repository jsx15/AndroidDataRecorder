using System;

namespace AndroidDataRecorder.Misc
{
    public class ResIntensList
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
        /// The cpu usage of the process
        /// </summary>
        public double Cpu { get; set; }
        
        /// <summary>
        /// The memory usage of the process
        /// </summary>
        public double Memory { get; set; }
        
        /// <summary>
        /// The process
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// The timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Empty constructor
        /// </summary>
        public ResIntensList(){}

        /// <summary>
        /// Constructor to set the variables
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="deviceName"></param>
        /// <param name="cpu"></param>
        /// <param name="memory"></param>
        /// <param name="process"></param>
        /// <param name="timestamp"></param>
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