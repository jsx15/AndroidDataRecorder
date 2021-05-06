using System;
using System.IO;
using System.Linq;
using AndroidDataRecorder.Backend;

namespace AndroidDataRecorder.Screenrecord
{
    public static class VideoPath
    {
        /// <summary>
        /// create directory to store videos in it
        /// </summary>
        /// <param name="deviceSerial">device identifier</param>
        /// <returns>path of created directory</returns>
        public static string Create(string deviceSerial)
        {
            // Replace all Invalid filename chars with _
            deviceSerial = Path.GetInvalidFileNameChars().Aggregate(deviceSerial, (current, c) => current.Replace(c, '_'));
            
            // directory to create
            var path = Config.GetVideoDirPath + deviceSerial + Path.DirectorySeparatorChar;
            
            //try to create a path
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    //return already existing directory
                    return path;
                }
                // Try to create the directory.
                Directory.CreateDirectory(path);
                
                //return created path
                return path;
            }
            //catch Exception
            catch (Exception)
            {
                //print exception
                Console.WriteLine("Could not create file ... continuing");
            }
            //return null if failed
            return null;
        }
    }
}