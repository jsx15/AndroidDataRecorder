using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.Extensions.WebEncoders.Testing;

namespace AndroidDataRecorder.Misc
{
    public class Filter
    { 
        /*
         * Marker
         */
        public Marker marker { get; set; }

        /*
         * The Loglevel
         */
        public string Level { get; set; }

        /*
         * TimeSpan for the marker --> plus or minus minutes
         */
        public double timeSpanMinus;
        public double timeSpanPlus;

        /*
         * List for LogEntries
         */
        public List<LogEntry> Logs = new List<LogEntry>();
    }
}