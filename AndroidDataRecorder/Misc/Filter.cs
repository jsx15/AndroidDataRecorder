using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.WebEncoders.Testing;

namespace AndroidDataRecorder.Misc
{
    public class Filter
    { 
        /// <summary>
        /// Marker
        /// </summary>
        public Marker marker { get; set; }

        /// <summary>
        /// Loglevel
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// TimeSpan for the marker --> plus or minus minutes
        /// </summary>
        public double timeSpanMinus;
        public double timeSpanPlus;

        /// <summary>
        /// List for LogEntries
        /// </summary>
        public List<LogEntry> Logs = new List<LogEntry>();

        public List<ResourcesList> Resources = new List<ResourcesList>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mar">Marker to set in Filter</param>
        public Filter(Marker mar)
        {
         marker = mar;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Filter()
        {
         marker = new Marker();
        }
    }
}