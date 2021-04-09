using System;
using System.Collections.Generic;
using System.IO;

namespace AndroidDataRecorder.Screenrecord
{
    public static class MarkerVideo
    {
        public static List<string> GetVideoFiles(DateTime markerTime, string path, int videoLength, int replayLength)
        {
            string[] files = Directory.GetFiles(path);
            List<string> fileList = new List<string>();
            Console.WriteLine(markerTime);
            foreach (string file in files)
            {
                Console.WriteLine(file);
                FileInfo fileInfo = new FileInfo(file);
                Console.WriteLine(fileInfo.CreationTime);
                if (fileInfo.CreationTime < markerTime.AddMilliseconds(videoLength + 1000) && fileInfo.CreationTime > markerTime.AddMilliseconds(-replayLength))
                {
                    fileList.Add(file);
                }
            }
            return fileList;
        }
    }
}