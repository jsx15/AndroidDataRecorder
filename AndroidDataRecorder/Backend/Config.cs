using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SharpAdbClient;

namespace AndroidDataRecorder.Backend
{
    public static class Config
    {
        /// <summary>
        /// The path to the config.json file
        /// </summary>
        private static readonly string Path = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"src" + System.IO.Path.DirectorySeparatorChar + "config.json"));

        /// <summary>
        /// The Source class object
        /// </summary>
        private static Source _source;

        /// <summary>
        /// Bool variable to check if connected to the AdbServer
        /// </summary>
        private static bool _connected;
        
        /// <summary>
        /// Load the config.json file into the Source class object
        /// Initialize the ADBServer with the given path to the adb.exe
        /// Connect to known devices
        /// </summary>
        public static void LoadConfig()
        {
            _source = JsonConvert.DeserializeObject<Source>(File.ReadAllText(Path, Encoding.Default));

            while (!_connected)
            {
                try
                {
                    if (_source != null) AdbServer.InitializeAdbServer(_source.AdbPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("No valid path to the adb.exe \nPlease define one: \n");
                    if (_source != null) _source.AdbPath = Console.ReadLine();
                }
                
                _connected = true;
            }

            ConnectKnownDevices();
        }

        /// <summary>
        /// Write the content of the source class object into the config.json file
        /// </summary>
        public static void SaveConfig()
        {
            var jsonResult = JsonConvert.SerializeObject(_source);
            using (StreamWriter w = File.CreateText(Path))
            {
                w.WriteLine(jsonResult);
            }
        }

        /// <summary>
        /// Connect to already known devices
        /// </summary>
        private static void ConnectKnownDevices()
        {
            try
            {
                for (int i = 0; i < _source.KnownDevices.Count - 1; i++)
                {
                    AdbServer.ConnectWirelessClient(_source.KnownDevices[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        /// <summary>
        /// Get the path to the adb.exe
        /// </summary>
        /// <returns> The path to adb.exe </returns>
        public static string GetAdbPath() => _source.AdbPath;

        /// <summary>
        /// Get the path to the ffmpeg.exe
        /// </summary>
        /// <returns> The path to the ffmpeg.exe </returns>
        public static string GetFfmpegPath() => _source.FfmpegPath;

        /// <summary>
        /// Get the path to the video directory
        /// </summary>
        /// <returns> The path to the video directory </returns>
        public static string GetVideoDirPath => _source.VideoDirPath;

        /// <summary>
        /// Get jira server url
        /// </summary>
        /// <returns>url of jira server</returns>
        public static string GetJiraServerUrl() => _source.JiraServerUrl;

        /// <summary>
        /// Get jira username
        /// </summary>
        /// <returns>username for jira</returns>
        public static string GetJiraUsername() => _source.JiraUsername;

        /// <summary>
        /// Get api token
        /// </summary>
        /// <returns>api token for jira</returns>
        public static string GetApiToken() => _source.ApiToken;

        /// <summary>
        /// Gets the accessWorkloadInterval and convert it to int. Then returns it in milliseconds
        /// </summary>
        /// <returns> Returns the value but if not possible returns a default interval of 5000 ms </returns>
        public static int GetAccessWorkloadInterval()
        {
            try
            {
                var interval = Int32.Parse(_source.AccessWorkloadInterval);
                //The interval should be between 1 and 60 seconds
                if (interval > 0 && interval <= 60) return 1000 * interval;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            return 5000;
        }

        /// <summary>
        /// Change the value in adbPath
        /// </summary>
        /// <param name="path"> The path to the adb.exe </param>
        public static void ChangeAdbPath(string path)
        {
            _source.AdbPath = path;
            SaveConfig();
        }
        
        /// <summary>
        /// Change the value in ffmpegPath
        /// </summary>
        /// <param name="path"> The path to the ffmpeg.exe </param>
        public static void ChangeFfMpegPath(string path)
        {
            _source.FfmpegPath = path;
            SaveConfig();
        }

        /// <summary>
        /// Change the value of videoDirPath
        /// </summary>
        /// <param name="path"> The path to the video directory </param>
        public static void ChangeVideoDirPath(string path)
        {
            _source.VideoDirPath = path;
            SaveConfig();
        }
        
        /// <summary>
        /// Change the value of JiraServerUrl
        /// </summary>
        /// <param name="url"> the url of the Jira server </param>
        public static void ChangeJiraServerUrl(string url)
        {
            _source.JiraServerUrl = url;
            SaveConfig();
        }
        
        /// <summary>
        /// Change the value of JiraUsername
        /// </summary>
        /// <param name="username"> The username for Jira </param>
        public static void ChangeJiraUsername(string username)
        {
            _source.JiraUsername = username;
            SaveConfig();
        }
        
        /// <summary>
        /// Change the value of ApiToken
        /// </summary>
        /// <param name="apiToken"> The api token of Jira </param>
        public static void ChangeApiToken(string apiToken)
        {
            _source.ApiToken = apiToken;
            SaveConfig();
        }

        /// <summary>
        /// Change the value of accessWorkloadInterval
        /// </summary>
        /// <param name="accessWorkloadInterval"> The interval at which the cpu/mem usage is accessed </param>
        public static void ChangeAccessWorkloadInterval(int accessWorkloadInterval)
        {
            try
            {
                _source.AccessWorkloadInterval = accessWorkloadInterval.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            SaveConfig();
        }
        
        /// <summary>
        /// Add a device to the knownDevices list
        /// </summary>
        /// <param name="ipAddress"> The ip address of the device </param>
        public static void AddKnownDevice(string ipAddress)
        {
            _source.KnownDevices.Add(ipAddress);
            SaveConfig();
        }
        
        /// <summary>
        /// Remove a device from the knownDevices list via index
        /// </summary>
        /// <param name="index"> The index of the device </param>
        public static void DeleteKnownDevice(int index)
        {
            _source.KnownDevices.RemoveAt(index);
            SaveConfig();
        }

        /// <summary>
        /// Remove a device from the knownDevices list via address
        /// </summary>
        /// <param name="address"> The address of the device </param>
        public static void DeleteKnownDevice(string address)
        {
            _source.KnownDevices.Remove(address);
            SaveConfig();
        }

        /// <summary>
        /// Clear the whole knownDevices list
        /// </summary>
        public static void ClearKnownDevices()
        {
            _source.KnownDevices.Clear();
            SaveConfig();
        }

        /// <summary>
        /// Get the list of known devices
        /// </summary>
        /// <returns> The list of known devices </returns>
        public static List<string> GetKnownDevices() => _source.KnownDevices;

        /// <summary>
        /// The representation of the config.json file as a c# class
        /// </summary>
        private class Source
        {
            public string AdbPath { get; set; }
            public string FfmpegPath { get; set; }
            public string VideoDirPath { get; set; }
            public string JiraServerUrl { get; set; }
            public string JiraUsername { get; set; }
            public string ApiToken { get; set; }
            public string AccessWorkloadInterval { get; set; }
            public List<string> KnownDevices { get; set; }
        }
    }
}