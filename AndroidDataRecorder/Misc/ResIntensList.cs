using System;

namespace AndroidDataRecorder.Misc
{
    public class ResIntensList
    {
        public double cpu { get; set; }
        public double memory { get; set; }
        public string process { get; set; }

        public DateTime timestamp { get; set; }
        
        public ResIntensList(){}

        public ResIntensList(double _cpu, double _memory, string _process, DateTime _timestamp)
        {
            cpu = _cpu;
            memory = _memory;
            process = _process;
            timestamp = _timestamp;
        }
    }
}