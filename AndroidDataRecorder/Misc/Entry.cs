using System;
using AndroidDataRecorder.Screenrecord;

namespace AndroidDataRecorder.Misc
{
    public class Entry
    {
        /// <summary>
        /// Name of source
        /// </summary>
        public string devicename;

        /// <summary>
        /// Message inside the entry
        /// </summary>
        public string message;

        /// <summary>
        /// Timestamp of the entry
        /// </summary>
        public DateTime timeStamp;
    }
}