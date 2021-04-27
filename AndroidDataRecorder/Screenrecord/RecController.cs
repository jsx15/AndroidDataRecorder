using System;
using System.Collections.Generic;
using System.Threading;
using AndroidDataRecorder.Misc;
using SharpAdbClient;

namespace AndroidDataRecorder.Screenrecord
{
    public static class RecController
    {
        // dictionary so handle all running screen record devices
        private static readonly Dictionary<string, Screenrecord> RecordList = new Dictionary<string, Screenrecord>();

        //count variable to count running concat threads
        public static int ThreadCounter; 
        
        /// <summary>
        /// method to start screen recording for a special device
        /// </summary>
        /// <param name="device">device to start record with</param>
        /// <param name="videoLength">length of the separate video parts</param>
        /// <returns>Returns whether the video was started or whether data is missing </returns>
        public static bool StartScrRec(DeviceData device, int videoLength)
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
                Screenrecord tmpDevice = new Screenrecord(device, videoLength);
                
                //store screen record object in dictionary
                RecordList.Add(device.Serial, tmpDevice);
                
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
            if (RecordList.TryGetValue(device.Serial, out var obj))
            {
                //stop recording of this device
                obj.StopRecording();
                
                //remove device from list
                RecordList.Remove(device.Serial);
            }
        }

        /// <summary>
        /// method to get status whether screen recording is running
        /// </summary>
        /// <param name="device">device to get status whether screen recording is running</param>
        /// <returns></returns>
        public static bool IsRecording(DeviceData device)
        {
            //find device in dictionary
            return RecordList.ContainsKey(device.Serial);
        }

        /// <summary>
        /// method to create a playable video files of separate unplayable files in timespan
        /// </summary>
        /// <param name="marker">marker with range around it</param>
        /// <param name="startTime">start of the time range</param>
        /// <param name="endTime">end of the time range</param>
        /// <param name="videoLength">video length of the separate video parts</param>
        public static void StartCreatingVideo(Marker marker, DateTime startTime, DateTime endTime, int videoLength)
        {
            //count new up coming thread
            ThreadCounter++;
            
            //Thread to start video creation
            new Thread (()=> MarkerVideo.CreateVideo(marker.devicename,marker.deviceSerial,startTime,endTime,marker.MarkerId,videoLength)).Start();
            
            Console.WriteLine("Thread started to create concatenate video");
        }
        
    }
}