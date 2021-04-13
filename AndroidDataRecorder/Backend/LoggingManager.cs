using System.Collections.Generic;
using System.Threading;

namespace AndroidDataRecorder.Backend
{
    public static class LoggingManager
    {
        /// <summary>
        /// The dictionary that contains the devices and their CancellationTokens
        /// </summary>
        private static readonly Dictionary<string, CancellationTokenSource> ThreadDictionary = new Dictionary<string, CancellationTokenSource>();

        /// <summary>
        /// Add a device to the dictionary
        /// </summary>
        /// <param name="serialNumber"> The serial number of the device </param>
        /// <param name="cts"> The CancellationToken for the device </param>
        public static void AddEntry(string serialNumber, CancellationTokenSource cts)
        {
            ThreadDictionary.Add(serialNumber, cts);
        }

        /// <summary>
        /// Delete a device from the entry and cancel the CancellationToken
        /// </summary>
        /// <param name="serialNumber"> the serial number of the device </param>
        /// <returns> true for success and false for failure </returns>
        public static bool DeleteEntry(string serialNumber)
        {
            if(ThreadDictionary.TryGetValue(serialNumber, out var cts))
            {
                cts.Cancel();
                Thread.Sleep(1000);
                cts.Dispose();
                ThreadDictionary.Remove(serialNumber);

                return true;
            }

            return false;
        }
    }
}