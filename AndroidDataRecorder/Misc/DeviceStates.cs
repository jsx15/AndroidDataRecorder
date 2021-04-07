using System;
using SharpAdbClient;

namespace AndroidDataRecorder.Misc
{
    public static class DeviceStates
    {
        /// <summary>
        /// Get connection type
        /// </summary>
        /// <param name="device">device to test</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ConnectionTypes ConnectionType(DeviceData device)
        {
            if (device is not null)
            {
                if (string.IsNullOrEmpty(device.Usb))
                {
                    return ConnectionTypes.Wifi ;
                }

                return ConnectionTypes.USB;
            }

            throw new Exception("Device is null");
        }
        
        /// <summary>
        /// Get ip address of device
        /// </summary>
        /// <param name="device">device to parse</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string IpAddress(DeviceData device)
        {
            if (ConnectionType(device).Equals(ConnectionTypes.Wifi))
            {
                if (device.ToString().Contains(':'))
                {
                    string[] split = device.ToString().Split(':');
                    return split[0];
                }

                return "IP Address could not be determined";
            }

            throw new Exception("Device is connected via USB");
        }
        
        public enum ConnectionTypes
        {
            Wifi,
            USB
        }
    }
}