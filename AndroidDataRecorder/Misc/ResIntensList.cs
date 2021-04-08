using System;

namespace AndroidDataRecorder.Misc
{
    public class ResIntensList
    {
        public int cpu { get; set; }
        public int memory { get; set; }
        public string app { get; set; }

        public DateTime timestamp { get; set; }
        
        public ResIntensList(){}

        public ResIntensList(int _cpu, int _memory, string _app, DateTime _timestamp)
        {
            cpu = _cpu;
            memory = _memory;
            app = _app;
            timestamp = _timestamp;
        }
    }
}