using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AndroidDataRecorder.Backend
{
    public static class Config
    {
        /// <summary>
        /// The path to the config.json file
        /// </summary>
        private static string _path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"src\config.json"));

        /// <summary>
        /// The Source class object
        /// </summary>
        private static Source _source;

        /// <summary>
        /// Bool variable to check if connected to the AdbServer
        /// </summary>
        private static bool connected = false;
        
        /// <summary>
        /// Load the config.json file into the Source class object
        /// Initialize the ADBServer with the given path to the adb.exe
        /// Connect to known devices
        /// </summary>
        public static void loadConfig()
        {
            _source = JsonConvert.DeserializeObject<Source>(File.ReadAllText(_path, Encoding.Default));

            while (!connected)
            {
                try
                {
                    ADBServer.initializeADBServer(_source.adbPath);
                    connected = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("No valid path to the adb.exe \n Please define one: \n");
                    _source.adbPath = Console.ReadLine();
                }
            }

            connectKnownDevices();
        }

        /// <summary>
        /// Write the content of the source class object into the config.json file
        /// </summary>
        public static void saveConfig()
        {
            var jsonResult = JsonConvert.SerializeObject(_source);
            using (StreamWriter w = File.CreateText(_path))
            {
                w.WriteLine(jsonResult);
            }
        }
        
        /// <summary>
        /// Connect to already known devices
        /// </summary>
        private static void connectKnownDevices()
        {
            try
            {
                for (int i = 0; i < _source.knownDevices.Count - 1; i++)
                {
                    ADBServer.ConnectWirelessCLient(_source.knownDevices[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// The representation of the config.json file as a c# class
        /// </summary>
        private class Source
        {
            public string adbPath { get; set; }
            public string ffmpegPath { get; set; }
            public List<string> knownDevices { get; set; }
            public Dictionary<string, bool> recordingDevices { get; set; }
        }
    }
}