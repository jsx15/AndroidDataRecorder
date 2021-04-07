using System;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Backend
{
    public class LogEntry : Entry
    {
        /// <summary>
        /// Devices Timestamp at Log occurence, since Device and System dont have to share the same system time
        /// </summary>
        public DateTime DeviceTimestamp { get; set; }
        
        /// <summary>
        /// Process Identifier
        /// </summary>
        public int Pid{ get; set; }
        
        /// <summary>
        /// Thread Identifier
        /// </summary>
        public int Tid{ get; set; }
        
        /// <summary>
        /// Log level 
        /// </summary>
        public string LogLevel{ get; set; }
        
        /// <summary>
        /// App that produced the log
        /// </summary>
        public string App{ get; set; }
        
        
        /// <summary>
        /// Constructor of a LogEntry
        /// </summary>
        /// <param name="deviceName">Name</param>
        /// <param name="systemTimestamp">Timestamp of System running AndroidDataRecorder</param>
        /// <param name="deviceTimestamp">Devices Timestamp at Log occurence, since Device and System dont have to share the same system time</param>
        /// <param name="pid">Process identifier</param>
        /// <param name="tid">Thread identifier (can be the same as Process identifier if there’s only one thread)</param>
        /// <param name="logLevel">Log priority</param>
        /// <param name="app">App that produced the log</param>
        /// <param name="logMessage">Message from the log</param>
        public LogEntry(string deviceName, DateTime systemTimestamp, DateTime deviceTimestamp, 
            int pid, int tid, string logLevel, string app, string logMessage)
        {
            devicename = deviceName;
            timeStamp = systemTimestamp;
            DeviceTimestamp = deviceTimestamp;
            Pid = pid;
            Tid = tid;
            LogLevel = logLevel;
            App = app;
            message = logMessage;
        }
        
    }
}