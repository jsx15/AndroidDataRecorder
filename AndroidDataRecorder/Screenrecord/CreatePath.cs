using System;
using System.IO;

namespace AndroidDataRecorder.Screenrecord
{
    public static class CreatePath
    {
        /// <summary>
        /// create directory to store videos in it
        /// </summary>
        /// <returns>path of created directory</returns>
        public static string HandlePath()
        {
            // directory to create
            var path = @"C:\Users\robin\Desktop\Screenrecord_"+Timestamp.GetTimestamp()+@"\";

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