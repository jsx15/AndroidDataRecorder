using System;
using AndroidDataRecorder.Screenrecord;

namespace AndroidDataRecorder.Misc
{
    public class Entry
    {
        /// <summary>
        /// Name of source
        /// </summary>
        public string devicename { get; set; }
        
        /// <summary>
        /// Serialnumber of the device
        /// </summary>
        public string deviceSerial { get; set; }

        /// <summary>
        /// Message inside the entry
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Timestamp of the entry
        /// </summary>
        public DateTime timeStamp { get; set; }
    }
}