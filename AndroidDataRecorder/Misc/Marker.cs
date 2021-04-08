using System;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace AndroidDataRecorder.Misc
{
    public class Marker : Entry
    {

        public int markerId { set; get; }
        public Marker(){}

        public Marker(string deviceName, DateTime time, string message)
        {
            base.devicename = deviceName;
            base.timeStamp = time;
            base.message = message;
        }
        
        public Marker(int markerId, string deviceName, DateTime time, string message)
        {
            this.markerId = markerId;
            base.devicename = deviceName;
            base.timeStamp = time;
            base.message = message;
        }
    }

}