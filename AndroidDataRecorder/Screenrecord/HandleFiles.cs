using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AndroidDataRecorder.Backend;

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
        /// concatenate all existing videos to one playable video file
        /// </summary>
        /// <param name="list">list of existing video files</param>
        /// <param name="videoPath">directory in which the files are to be searched for</param>
        /// <param name="listFile"></param>
        /// <param name="videoName">name of the finish video</param>
        public static void ConcVideoFiles(List<string> list, string videoPath, string listFile, string videoName)
        {
            //create process for ffmpeg
            var ffmpeg = new Process
            {
                StartInfo =
                {
                    //path to ffmpeg.exe
                    FileName = Config.GetFfmpegPath(),
                    //set arguments for concatenating all files in list.txt
                    Arguments = @"-f concat -safe 0 -i " + listFile + " -c copy " +
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

        /// <summary>
        /// check if file is used by a process
        /// </summary>
        /// <param name="file">file path</param>
        /// <returns>true if it is in use -> false if not</returns>
        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                //try to open and read a file
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    //close stream if it was possible
                    stream.Close();
                }
            } //if file i in use -> catch exception
            catch (IOException)
            {
                //return true, because file is in use
                return true;
            }

            //return false, because file is not in use
            return false;
        }
    }
}