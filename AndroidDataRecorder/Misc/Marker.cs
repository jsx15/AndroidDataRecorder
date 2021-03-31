using Microsoft.AspNetCore.SignalR.Protocol;

namespace AndroidDataRecorder.Misc
{
    public class Marker
    {
        public string _markerTime { get; set; }
        public string _markerMessage { get; set; }

        public Marker(string time, string message)
        {
            _markerMessage = message;
            _markerTime = time;
        }
    }
}