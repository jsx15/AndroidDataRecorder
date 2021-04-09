using System;
using System.Collections.Generic;
using System.IO;
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

        public static bool CeateVideo(DateTime markerTime, DeviceData deviceData, int videoLength,int replayLength, int markerId)
        {
            string videoPath = Config.GetVideoDirPath + deviceData + Path.DirectorySeparatorChar;
            string textFilePath = videoPath + "list_marker_" + markerId + ".txt";
            List<string> fileList = MarkerVideo.GetVideoFiles(markerTime, videoPath, videoLength, replayLength);
            if (fileList.Count == 0) return false;
            Console.WriteLine(fileList[^1]);
            FileInfo fileInfo = new FileInfo(fileList[^1]);
            while (IsFileLocked(fileInfo))
            {
                Console.WriteLine("wait");
            }
            
            HandleFiles.ConcVideoFiles(fileList,Config.GetVideoDirPath, textFilePath,"marker_"+markerId+"_hero2lte",markerId);
            HandleFiles.DeleteFile(textFilePath);
            return true;
        }
        
        private static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using(FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
    }
}