using System;
using System.Diagnostics;
using AndroidDataRecorder.Backend;
using SharpAdbClient;
using AdbServer = AndroidDataRecorder.Backend.AdbServer;

namespace AndroidDataRecorder.Screenrecord
{
    public class Function
    {
        public static bool ScrRecAvailable(DeviceData device)
        {
            var receiver = new ConsoleOutputReceiver();
            AdbServer.GetClient().ExecuteRemoteCommand("ls /system/bin/", device, receiver);
            var received = receiver.ToString();
            var available = received.Contains("screenrecord");
            Console.WriteLine("Screenrecord function available: " + available);
            return available;
        }
    }
}