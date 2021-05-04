using System;

namespace AndroidDataRecorder.Misc
{
    public class Entry
    {
        /// <summary>
        /// Name of source
        /// </summary>
        public string Devicename { get; set; }
        
        /// <summary>
        /// Serialnumber of the device
        /// </summary>
        public string DeviceSerial { get; set; }

        /// <summary>
        /// Message inside the entry
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Timestamp of the entry
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}