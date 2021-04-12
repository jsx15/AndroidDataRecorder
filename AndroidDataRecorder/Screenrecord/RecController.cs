using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AndroidDataRecorder.Backend;
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

        public static void StartCreatingVideo(DateTime markerTime, DeviceData deviceData, int videoLength,
            int replayLength, int markerId)
        {
            new Thread (()=> CreateVideo(markerTime,deviceData,videoLength,replayLength,markerId)).Start();
        }

        /// <summary>
        /// find all necessary files
        /// concat these video files
        /// delete text file
        /// </summary>
        /// <param name="markerTime">time of set marker</param>
        /// <param name="deviceData">device from which the video is to be taken </param>
        /// <param name="videoLength">length of the separate video parts</param>
        /// <param name="replayLength">length of the resulting video</param>
        /// <param name="markerId">marker ID</param>
        /// <returns></returns>
        private static void CreateVideo(DateTime markerTime, DeviceData deviceData, int videoLength,int replayLength, int markerId)
        {
            //create path of the video files
            var videoPath = Config.GetVideoDirPath + deviceData + Path.DirectorySeparatorChar;

            //create resulting video name
            var videoName = "marker_" + markerId + deviceData.Name;
            
            //create path of the text file
            var textFilePath = videoPath + "list_marker_" + markerId + ".txt";
            
            //get all files in the time range
            var fileList = MarkerVideo.GetVideoFiles(markerTime, videoPath, videoLength, replayLength);
            
            //check if there is a file in this range
            if (fileList.Count == 0) return;
            
            Console.WriteLine(fileList[^1]);
            
            //get last or newest file of this list
            var fileInfo = new FileInfo(fileList[^1]);
            
            //wait till recording of this file is done
            while (HandleFiles.IsFileLocked(fileInfo)) { }
            
            //concatenate all video parts
            HandleFiles.ConcVideoFiles(fileList,Config.GetVideoDirPath, textFilePath,videoName);
            
            //delete text file
            HandleFiles.DeleteFile(textFilePath);
        }
    }
}