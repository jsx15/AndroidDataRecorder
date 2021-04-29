using System;
using SharpAdbClient;
using AdbServer = AndroidDataRecorder.Backend.AdbServer;

namespace AndroidDataRecorder.Screenrecord
{
    public class Function
    {
        /// <summary>
        /// Check if screenrecord function is available -> Huawei does not have this function anymore
        /// </summary>
        /// <param name="device">Check if this device has function screenrecord function</param>
        /// <returns>TRUE if usable, FALSE if not usable</returns>
        public static bool ScrRecAvailable(DeviceData device)
        {
            //create receiver
            var receiver = new ConsoleOutputReceiver();
            
            //execute command and read input
            AdbServer.GetClient().ExecuteRemoteCommand("ls /system/bin/", device, receiver);
            
            //convert input to string
            var received = receiver.ToString();
            
            //check if screenrecord is listed
            var usable = received.Contains("screenrecord");
            
            
            Console.WriteLine("Screenrecord function usable: " + usable);
            
            //return if screenrecord is usable or not
            return usable;
        }
    }
}