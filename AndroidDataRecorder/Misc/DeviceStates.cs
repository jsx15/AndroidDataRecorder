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

        public enum ConnectionTypes
        {
            Wifi,
            USB
        }
    }
}