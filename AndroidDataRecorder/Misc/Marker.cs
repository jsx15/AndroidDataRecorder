using System;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace AndroidDataRecorder.Misc
{
    public class Marker : Entry
    {

        public Marker(){}

        public Marker(string deviceName, DateTime time, string message)
        {
            base.devicename = deviceName;
            base.timeStamp = time;
            base.message = message;
        }
    }

}