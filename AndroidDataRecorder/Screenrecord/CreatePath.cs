using System;
using System.IO;
using AndroidDataRecorder.Backend;

namespace AndroidDataRecorder.Screenrecord
{
    public static class CreatePath
    {
        /// <summary>
        /// create directory to store videos in it
        /// </summary>
        /// <param name="deviceName">device name</param>
        /// <returns>path of created directory</returns>
        public static string HandlePath(string deviceName)
        {
            // directory to create
            var path = Config.GetVideoDirPath+"Screenrecord_"+deviceName+"_"+Timestamp.GetTimestamp()+Path.DirectorySeparatorChar;
            
            //try to create a path
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine("That path exists already.");
                    
                    //return already existing directory
                    return path;
                }
                // Try to create the directory.
                Directory.CreateDirectory(path);
                
                Console.WriteLine("Path created at: "+path);
                
                //return created path
                return path;
            }
            //catch Exception
            catch (Exception e)
            {
                //print exception
                Console.WriteLine("The process failed: {0}", e);
            }
            //return null if failed
            return null;
        }
    }
}