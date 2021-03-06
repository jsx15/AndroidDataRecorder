using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AndroidDataRecorder.Backend;

namespace AndroidDataRecorder.Screenrecord
{
    public static class MarkerVideo
    {
        /// <summary>
        /// find videos with creation time in range
        /// </summary>
        /// <param name="path">path to find the video files</param>
        /// <param name="startTime">start time of the range</param>
        /// <param name="endTime">end time of the range</param>
        /// <param name="videoLength">video length of the separate files</param>
        /// <returns></returns>
        private static List<string> GetVideoFiles(string path, DateTime startTime, DateTime endTime,
            int videoLength)
        {
            //create file list
            var fileList = new List<string>();
            
            //check if path exists -> if no something went wrong
            if (!Directory.Exists(path)) return fileList;
            
            //get all files from directory
            var files = Directory.GetFiles(path);
            
            //check all files of array
            foreach (var file in files)
            {
                //get file info of file
                var fileInfo = new FileInfo(file);
                
                //check if creation time of file is in range
                if (fileInfo.CreationTime < endTime.AddMilliseconds(videoLength + 1000) &&
                    fileInfo.CreationTime > startTime)
                {
                    //if it is in range add file to list
                    fileList.Add(file);
                }
            }

            //sort list (for linux)
            fileList.Sort();
            
            //return list of files in specified range
            return fileList;
        }

        /// <summary>
        /// find all necessary files
        /// concat these video files
        /// delete text file
        /// </summary>
        ///  <param name="deviceName">name of device</param>
        /// <param name="deviceSerial">serial number of device</param>
        /// <param name="startTime">start time of range to concatenate files</param>
        /// <param name="endTime">end time of range to concatenate files</param>
        /// <param name="markerId">marker ID</param>
        /// <param name="videoLength">length of the separate video parts</param>
        public static void CreateVideo(string deviceName, string deviceSerial, DateTime startTime,
            DateTime endTime, int markerId, int videoLength)
        {
            //Replace all Invalid filename chars with _
            deviceSerial = Path.GetInvalidFileNameChars().Aggregate(deviceSerial, (current, c) => current.Replace(c, '_'));
            
            //create path of the video files
            var videoPath = Config.GetVideoDirPath + deviceSerial + Path.DirectorySeparatorChar;

            //create resulting video name
            var videoName = "marker_" + markerId + "_" + deviceName;

            //create path of the text file
            var textFilePath = videoPath + "list_marker_" + markerId + ".txt";

            //get all files in the time range
            var fileList = GetVideoFiles(videoPath, startTime, endTime, videoLength);

            //check if there is a file in this range
            if (fileList.Count == 0)
            {
                //No files to concat -> Thread done
                RecController.ThreadCounter--;
                
                //go back
                return;
            }

            //check every file in time range if it´s in use
            for(var i = 0; i<fileList.Count;i++)
            {
                //wait while file in use
                while (Screenrecord.FilesInUse.Contains(fileList[i]))
                {
                }
                
                //check if file exists -> maybe timeout while recording
                if (!File.Exists(fileList[i])) fileList.Remove(fileList[i]);
            }

            //concatenate all video parts
            HandleFiles.ConcVideoFiles(fileList, Config.GetVideoDirPath, textFilePath, videoName);

            //delete text file
            HandleFiles.DeleteFile(textFilePath);

            //concat thread is done
            RecController.ThreadCounter--;
        }
    }
}