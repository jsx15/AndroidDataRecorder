using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AndroidDataRecorder.Backend;
using AndroidDataRecorder.Misc;
using SharpAdbClient;

namespace AndroidDataRecorder.Screenrecord
{
    public static class HandleFiles
    {
        /// <summary>
        /// delete all files that are not named "video.mp4"
        /// </summary>
        /// <param name="path">path to file which will be deleted</param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                // If file found, delete it    
                File.Delete(path);
            }
        }

        /// <summary>
        /// while recording the maximum video number is controlled here
        /// if video number is higher than "numOfVideos" -> delete the oldest video
        /// </summary>
        /// <param name="path">directory in which the files are to be searched for and deleted</param>
        /// <param name="numOfVideos">the maximum number of videos</param>
        /// <param name="fileList">list of files</param>
        public static void CheckVideoNumber(string path, int numOfVideos, List<string> fileList)
        {
            //order files and get the files to be deleted
            var files = new DirectoryInfo(path).EnumerateFiles()
                //order files by descending creation time
                .OrderByDescending(f => f.CreationTime)
                //skip the number of file to keep
                .Skip(numOfVideos)
                //add other files to list
                .ToList();

            //go through all of the files
            foreach (var file in files)
            {
                //delete the file
                file.Delete();
                //remove this file from fileList
                fileList.Remove(file.FullName);
            }
        }

        /// <summary>
        /// concatenate all existing videos to one playable video file
        /// </summary>
        /// <param name="list">list of existing video files</param>
        /// <param name="path">directory in which the files are to be searched for</param>
        /// <param name="videoName">name of the finish video</param>
        /// <param name="markerId">marker ID</param>
        public static void ConcVideoFiles(List<string> list, string videoPath, string listFile, string videoName,
            int markerId)
        {
            //create process for ffmpeg
            var ffmpeg = new Process
            {
                StartInfo =
                {
                    //path to ffmpeg.exe
                    FileName = Config.GetFfmpegPath(),
                    //set arguments for concatenating all files in list.txt
                    Arguments = @"-f concat -safe 0 -i "+listFile+" -c copy " +
                                videoPath + videoName + ".mp4",
                    //redirect standard input
                    RedirectStandardInput = true,
                    //use not shell execute
                    UseShellExecute = false
                }
            };

            //store all files from list into an array
            var filesToMerge = list.ToArray();

            //check if text file already exists
            if (!File.Exists(listFile))
            {
                //create a stream writer
                using (var sw = File.CreateText(listFile))
                {
                    //list all files in the text file
                    foreach (var file in filesToMerge)
                    {
                        //write with syntax for ffmpeg
                        sw.WriteLine("file '" + file + "'");
                    }
                }
            }

            //start concatenate process
            ffmpeg.Start();

            //wait till process is finished
            ffmpeg.WaitForExit();
        }
    }
}