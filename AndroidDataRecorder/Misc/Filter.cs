using System.Collections.Generic;

namespace AndroidDataRecorder.Misc
{
    public class Filter
    { 
        /// <summary>
        /// Marker
        /// </summary>
        public Marker Marker { get; set; }

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

        /// <summary>
        /// Resources usage of device
        /// </summary>
        public List<ResourcesList> Resources = new List<ResourcesList>();

        /// <summary>
        /// Serial of device
        /// Initialization is empty for blazor dropdown
        /// </summary>
        public string DeviceSerial = "";
        
        /// <summary>
        /// Bool for video
        /// </summary>
        public bool CreateVideo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mar">Marker to set in Filter</param>
        /// <<param name="deviceSerial">serial of the device</param>
        public Filter(Marker mar, string deviceSerial)
        {
         Marker = mar;
         DeviceSerial = deviceSerial;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Filter()
        {
         Marker = new Marker();
        }
    }
}