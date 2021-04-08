using System;
using System.Collections.Generic;
using SharpAdbClient;

namespace AndroidDataRecorder.Screenrecord
{
    public static class RecController
    {
        // dictionary so handle all running screen record devices
        private static readonly Dictionary<string, Screenrecord> RecordList = new Dictionary<string, Screenrecord>();

        /// <summary>
        /// method to start screen recording for a special device
        /// </summary>
        /// <param name="device">device to start record with</param>
        /// <param name="videoLength">length of the separate video parts</param>
        /// <param name="numOfVideos">maximum number of videos to concatenate</param>
        /// <returns>Returns whether the video was started or whether data is missing </returns>
        public static bool StartScrRec(DeviceData device, int videoLength, int numOfVideos)
        {
            //check if all important data available
            if (device == null || device.Name.Equals("")) return false;
            
            //check if screen recording for this device is already running
            if (IsRecording(device))
            {
                Console.WriteLine("Recording is already running for the device " + device.Name);
            }
            else
            {
                //create new screen record object with specified settings
                Screenrecord tmpDevice = new Screenrecord(device, videoLength,numOfVideos);
                
                //store screen record object in dictionary
                RecordList.Add(device.ToString(), tmpDevice);
                
                //start screen recording for this device
                tmpDevice.StartScreenrecord();
            }
            //return true -> recording is running
            return true;
        }

        /// <summary>
        /// method to stop screen recording for a special device
        /// </summary>
        /// <param name="device">device to stop record with</param>
        public static void StopScrRec(DeviceData device)
        {
            //find device in dictionary
            if (RecordList.TryGetValue(device.ToString(), out var obj))
            {
                //stop recording of this device
                obj.StopRecording();
                
                //remove device from list
                RecordList.Remove(device.ToString());
            }
        }

        /// <summary>
        /// method to get status whether screen recording is running
        /// </summary>
        /// <param name="device">device to get status whether screen recording is running</param>
        /// <returns></returns>
        private static bool IsRecording(DeviceData device)
        {
            //find device in dictionary
            return RecordList.ContainsKey(device.ToString());
        }
    }
}