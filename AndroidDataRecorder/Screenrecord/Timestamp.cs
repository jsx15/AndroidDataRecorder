using System;

namespace AndroidDataRecorder.Screenrecord
{
    public class Timestamp
    {
        public static String GetTimestamp()
        {
            return DateTime.Now.ToString("MM-dd-yy_HH-mm-ss");
        }
    }
}