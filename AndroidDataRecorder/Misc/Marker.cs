using System;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace AndroidDataRecorder.Misc
{
    public class Marker
    {
        public string _deviceName { get; set; }

        public DateTime _markerTimestamp { get; set; }
        public string _markerMessage { get; set; }
        
        public Marker(){}
        
        

        public Marker(string deviceName, DateTime time, string message)
        {
            _deviceName = deviceName;
            _markerTimestamp = time;
            _markerMessage = message;
        }
    }

}