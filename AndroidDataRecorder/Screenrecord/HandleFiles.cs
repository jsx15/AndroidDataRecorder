using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AndroidDataRecorder.Backend;

namespace AndroidDataRecorder.Screenrecord
{
    public static class HandleFiles
    {
        /// <summary>
        /// delete all files that are not named "video.mp4"
        /// </summary>
        /// <param name="path">directory in which the files are to be searched for and deleted</param>
        public static void DeleteOldFiles(string path)
        {
            //get an array of files of path
            var filePaths = Directory.GetFiles(path);
            
            //check every file if name is "video.mp4"
            foreach (var filePath in filePaths)
            {
                //get name of this file
                var name = new FileInfo(filePath).Name;
                
                //convert name to lower case 
                name = name.ToLower();
                
                //check if the files name is "video.mp4"
                if (name != "video.mp4")
                {
                    //if name is not "video.mp4" -> delete it
                    File.Delete(filePath);
                }
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
        public static void ConcVideoFiles(List<string> list, string path)
        {
            //create process for ffmpeg
            var ffmpeg = new Process
            {
                StartInfo =
                {
                    //path to ffmpeg.exe
                    FileName = Config.GetFfmpegPath(),
                    //set arguments for concatenating all files in list.txt
                    Arguments = @"-f concat -safe 0 -i "+path+"list.txt -c copy "+path+"video.mp4",
                    //redirect standard input
                    RedirectStandardInput = true,
                    //use not shell execute
                    UseShellExecute = false
                }
            };

            //store all files from list into an array
            var filesToMerge = list.ToArray();
            
            //create path for text file
            var textPath = path+"list.txt";
            
            //check if text file already exists
            if (!File.Exists(textPath))
            {
                //create a stream writer
                using (var sw = File.CreateText(textPath))
                {
                    //list all files in the text file
                    foreach (var file in filesToMerge)
                    {
                        //write with syntax for ffmpeg
                        sw.WriteLine("file '"+file+"'");
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