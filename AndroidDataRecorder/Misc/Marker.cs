using System;
using System.ComponentModel;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace AndroidDataRecorder.Misc
{
    public class Marker : Entry
    {
        /// <summary>
        /// MarkerID
        /// </summary>
        public int MarkerId { set; get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceName">Name of device</param>
        /// <param name="time">Timestamp of the marker</param>
        /// <param name="message">Marker message</param>
        public Marker( string deviceName, DateTime time, string message)
        {
            base.devicename = deviceName;
            base.timeStamp = time;
            base.message = message;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serial">Serial number of device</param>
        /// <param name="markerId">ID of the marker</param>
        /// <param name="deviceName">Name of device</param>
        /// <param name="time">Timestamp of the marker</param>
        /// <param name="message">Marker message</param>
        public Marker(string serial, int markerId, string deviceName, DateTime time, string message)
        {
            this.MarkerId = markerId;
            base.deviceSerial = serial;
            base.devicename = deviceName;
            base.timeStamp = time;
            base.message = message;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Marker(){}

        /// <summary>
        /// Get marker id
        /// </summary>
        /// <returns>Marker id</returns>
        public  string GetMarkerId()
        {
            return MarkerId.ToString();
        }

        public override string ToString()
        {
            return "#"+ timeStamp +" MarkerID: " + MarkerId + "Message: " + message;
        }
    }

}