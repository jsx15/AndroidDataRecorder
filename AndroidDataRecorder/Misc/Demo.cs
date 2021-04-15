using System.Collections.Generic;

namespace AndroidDataRecorder.Misc
{
    public class Demo
    {
        public List<combinedInfo> allSelected = new List<combinedInfo>();
        public class combinedInfo
        {
            public Marker _marker;
            public double timeSpanMinus;
            public double timeSpanPlus;
            //public string level;
            public List<LogEntry> _logs = new List<LogEntry>();
        }
    }
}