using System;
using System.Collections.Generic;
using SharpAdbClient;

namespace AndroidDataRecorder.Screenrecord
{
    public static class RecController
    {
        private static readonly Dictionary<string, Screenrecord> RecordList = new Dictionary<string, Screenrecord>();

        public static bool StartScrRec(DeviceData device, int videoLength, int numOfVideos)
        {
            if (device == null || device.Name.Equals("")) return false;
            if (IsRecording(device))
            {
                Console.WriteLine("Recording is already running for the device " + device.Name);
            }
            else
            {
                Screenrecord tmpDevice = new Screenrecord(device, videoLength,numOfVideos);
                RecordList.Add(device.ToString(), tmpDevice);
                tmpDevice.StartScreenrecord();
            }
            return true;
        }

        public static void StopScrRec(DeviceData device)
        {
            if (RecordList.TryGetValue(device.ToString(), out var obj))
            {
                obj.StopRecording();
                RecordList.Remove(device.ToString());
            }
        }

        private static bool IsRecording(DeviceData device)
        {
            return RecordList.ContainsKey(device.ToString());
        }
    }
}