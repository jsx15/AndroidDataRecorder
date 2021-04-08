using System;
using System.Collections.Generic;
using SharpAdbClient;

namespace AndroidDataRecorder.Screenrecord
{
    public static class RecController
    {
        private static readonly Dictionary<string,Screenrecord> RecordList = new Dictionary<string,Screenrecord>();

        public static void StartScrRec(DeviceData device)
        {
            if (IsRecording(device))
            {
               Console.WriteLine("Recording is already running for the device " + device.Name); 
            }
            else
            {
                Screenrecord tmpDevice = new Screenrecord(device);
                RecordList.Add(device.Name,tmpDevice);
                tmpDevice.StartScreenrecord();
            }
        }

        public static void StopScrRec(DeviceData device)
        {
            if (RecordList.TryGetValue(device.Name, out var obj))
            {
                obj.StopRecording();
                RecordList.Remove(device.Name);
            }
        }

        private static bool IsRecording(DeviceData device)
        {
            return RecordList.ContainsKey(device.Name);
        }


    }
}