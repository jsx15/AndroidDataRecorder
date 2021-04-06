using System;

namespace AndroidDataRecorder.Misc
{
    public class Logs
    {
        public string _deviceName { get; set; }
        public DateTime _systemTimestamp { get; set; } 
        public DateTime _deviceTimestamp { get; set; }
        public int _pid { get; set; }
        public int _tid { get; set; }
        public string _loglevel { get; set; }
        public string _app { get; set; }
        public string _logMessage { get; set; }

        public Logs(){}

        public Logs(string deviceName, DateTime systemTimestamp, DateTime deviceTimestamp, int pid,
            int tid, string loglevel, string app, string logMessage)
        {
            _deviceName = deviceName;
            _systemTimestamp = systemTimestamp;
            _deviceTimestamp = deviceTimestamp;
            _pid = pid;
            _tid = tid;
            _loglevel = loglevel;
            _app = app;
            _logMessage = logMessage;

        }
    }
}