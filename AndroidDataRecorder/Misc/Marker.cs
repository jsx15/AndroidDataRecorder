using System;
using System.ComponentModel;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace AndroidDataRecorder.Misc
{
    [TypeConverter(typeof (MarkerConverter))]
    public class Marker : Entry
    {
        
        public int markerId { set; get; }
        public Marker(){}

        public Marker( string deviceName, DateTime time, string message)
        {
            base.devicename = deviceName;
            base.timeStamp = time;
            base.message = message;
        }
        
        public Marker(string serial, int markerId, string deviceName, DateTime time, string message)
        {
            this.markerId = markerId;
            base.deviceSerial = serial;
            base.devicename = deviceName;
            base.timeStamp = time;
            base.message = message;
        }
    }

}