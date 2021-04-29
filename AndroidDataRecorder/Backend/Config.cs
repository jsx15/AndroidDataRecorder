using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

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
        /// Load the config.json file into the Source class object
        /// Initialize the ADBServer with the given path to the adb.exe
        /// Connect to known devices
        /// </summary>
        public static void LoadConfig()
        {
            _source = JsonConvert.DeserializeObject<Source>(File.ReadAllText(Path, Encoding.Default));

            CheckFfmpegPath();
            CheckVideoDirPath();

            if (!ValidateJiraUsername() || !ValidateApiToken() || !ValidateJiraServerUrl())
            {
                Console.WriteLine("At least one of the Ticket creation variables is not set \nShould they be adjusted? [y/N]");
                if (Console.ReadLine()!.Equals("y"))
                {
                    CheckJiraServerUrl();
                    CheckJiraUsername();
                    CheckApiToken();
                }
            }
            
            CheckWorkloadInterval();
            CheckAdbPath();

            SaveConfig();

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
        /// Checks if the path to the adb.exe is correct and if not asks the user to type in a correct path
        /// </summary>
        private static void CheckAdbPath()
        {
            if (ValidateAdbPath()) return;
            do
            {
                Console.WriteLine("No valid path to the adb.exe \nPlease define one:");
                if (_source != null) _source.AdbPath = Console.ReadLine();
            } while (!ValidateAdbPath());
            
            Console.WriteLine("Success !!!");
            
            /*while (true)
            {
                try
                {
                    if (_source != null) AdbServer.InitializeAdbServer(_source.AdbPath);
                    Console.WriteLine("Success !!!");
                    return;
                }
                catch (Exception)
                {
                    Console.WriteLine("No valid path to the adb.exe \nPlease define one:");
                    if (_source != null) _source.AdbPath = Console.ReadLine();
                }
            }*/
        }

        /// <summary>
        /// Checks if the path to adb.exe is correct
        /// </summary>
        /// <returns> true if yes and false if not </returns>
        private static bool ValidateAdbPath()
        {
            try
            {
                if (_source != null) AdbServer.InitializeAdbServer(_source.AdbPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Checks if the path to the ffmpeg.exe is correct and if not asks the user to type in a correct path
        /// </summary>
        private static void CheckFfmpegPath()
        {
            if (File.Exists(_source.FfmpegPath)) return;
            do
            {
                Console.WriteLine("No valid path to the ffmpeg.exe \nPlease define one:");
                _source.FfmpegPath = Console.ReadLine();
            } while (!File.Exists(_source.FfmpegPath));
                
            Console.WriteLine("Success !!!");
            /*while (true)
            {
                if (!File.Exists(_source.FfmpegPath))
                {
                    Console.WriteLine("No valid path to the ffmpeg.exe \nPlease define one:");
                    _source.FfmpegPath = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Success !!!");
                    return;
                }
            }*/
        }
        
        /// <summary>
        /// Checks if the path to the video directory is correct and if not asks the user to type in a correct path
        /// </summary>
        private static void CheckVideoDirPath()
        {
            if (Directory.Exists(_source.VideoDirPath)) return;
            do
            {
                Console.WriteLine("No valid path to the video directory \nPlease define one:");
                _source.VideoDirPath = Console.ReadLine();
            } while (!Directory.Exists(_source.VideoDirPath));
                
            if (_source.VideoDirPath != null &&
                !_source.VideoDirPath.EndsWith(System.IO.Path.DirectorySeparatorChar))
            {
                _source.VideoDirPath += System.IO.Path.DirectorySeparatorChar;
            }
            Console.WriteLine("Success !!!");

            /*while (true)
            {
                if (!Directory.Exists(_source.VideoDirPath))
                {
                    Console.WriteLine("No valid path to the video directory \nPlease define one:");
                    _source.VideoDirPath = Console.ReadLine();
                }
                else
                {
                    if (_source.VideoDirPath != null &&
                        !_source.VideoDirPath.EndsWith(System.IO.Path.DirectorySeparatorChar))
                    {
                        _source.VideoDirPath += System.IO.Path.DirectorySeparatorChar;
                    }
                    Console.WriteLine("Success !!!");
                    return;
                }
            }*/
        }

        /// <summary>
        /// Checks if the URL to the Jira server is correct and if not asks the user to type in a correct URL
        /// </summary>
        private static void CheckJiraServerUrl()
        {
            if (ValidateJiraServerUrl()) return;
            do
            {
                Console.WriteLine("No valid URL to the Jira server \nPlease define one:");
                _source.JiraServerUrl = Console.ReadLine();
            } while (!ValidateJiraServerUrl());
            
            Console.WriteLine("Success !!!");
            
            /*while (true)
            {
                //Creating the HttpWebRequest and Setting the Request method HEAD
                if (ValidateJiraServerUrl())
                {
                    Console.WriteLine("Success !!!");
                    return;
                }
                
                Console.WriteLine("No valid URL to the Jira server \nPlease define one:");
                _source.JiraServerUrl = Console.ReadLine();
            }*/
        }

        /// <summary>
        /// Checks if the Jira Server Url is a correct  Url
        /// </summary>
        /// <returns> true if it is and false if not</returns>
        private static bool ValidateJiraServerUrl()
        {
            try
            {
                //Creating the HttpWebRequest and Setting the Request method HEAD
                if (WebRequest.Create(_source.JiraServerUrl!) is HttpWebRequest request)
                {
                    request.Method = "HEAD";
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    response?.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Checks if the Jira Username is empty and if yes asks the user to type in a Username
        /// </summary>
        private static void CheckJiraUsername()
        {
            if (ValidateJiraUsername()) return;
            do
            {
                Console.WriteLine("No valid Jira Username \nPlease define one:");
                _source.JiraUsername = Console.ReadLine();
            } while (!ValidateJiraUsername());
            
            Console.WriteLine("Success !!!");
            
            /*while (true)
            {
                if (ValidateJiraUsername())
                {
                    Console.WriteLine("Success !!!");
                    return;
                }
                
                Console.WriteLine("No valid Jira Username \nPlease define one:"); 
                _source.JiraUsername = Console.ReadLine();
            }*/
        }

        /// <summary>
        /// Checks if the Jira Username is in the form of a email address
        /// </summary>
        /// <returns> true if it is and false if not</returns>
        private static bool ValidateJiraUsername()
        {
            try
            {
                var unused = new System.Net.Mail.MailAddress(_source.JiraUsername!);
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Checks if the Jira Api Token is empty and if yes asks the user to type in a Api Token
        /// </summary>
        private static void CheckApiToken()
        {
            if (ValidateApiToken()) return;
            do
            {
                Console.WriteLine("No valid Api Token \nPlease define one:");
                _source.ApiToken = Console.ReadLine();
            } while (!ValidateApiToken());

            Console.WriteLine("Success !!!");
            
            /*while (true)
            {
                if (ValidateApiToken())
                {
                    Console.WriteLine("Success !!!");
                    return;
                }

                Console.WriteLine("No valid Api Token \nPlease define one:");
                _source.ApiToken = Console.ReadLine();
            }*/
        }
        
        /// <summary>
        /// Checks if the Api Token is set
        /// </summary>
        /// <returns> true if it is set and false if not</returns>
        private static bool ValidateApiToken()
        {
            if (_source.ApiToken != null && _source.ApiToken.Equals(""))
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Checks if the Access Workload Interval is between 1 and 60 if not asks the user to type in a new one
        /// </summary>
        private static void CheckWorkloadInterval()
        {
            if (ValidateWorkloadInterval()) return;
            do
            {
                Console.WriteLine("The Access Workload Interval must be between 1 and 60 seconds \nPlease define a new one:");
                _source.AccessWorkloadInterval = Console.ReadLine();
            } while (!ValidateWorkloadInterval());
            
            Console.WriteLine("Success !!!"); 
            
            /*while (true)
            {
                try
                {
                    var interval = Int32.Parse(_source.AccessWorkloadInterval!);
                    if (interval > 0 && interval <= 60)
                    {
                        Console.WriteLine("Success !!!"); 
                        return;
                    }
                    Console.WriteLine("The Access Workload Interval must be between 1 and 60 seconds \nPlease define a new one:");
                    _source.AccessWorkloadInterval = Console.ReadLine();
                }
                catch (Exception)
                {
                    Console.WriteLine("The Access Workload Interval must be between 1 and 60 seconds \nPlease define a new one:");
                    _source.AccessWorkloadInterval = Console.ReadLine();
                }
            }*/
        }

        /// <summary>
        /// Checks if the workload interval is set and in correct format 
        /// </summary>
        /// <returns> true if it is set and false if not </returns>
        private static bool ValidateWorkloadInterval()
        {
            try
            {
                var interval = Int32.Parse(_source.AccessWorkloadInterval!);
                return interval > 0 && interval <= 60;
            }
            catch (Exception)
            {
                return false;
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
            catch (Exception)
            {
                // ignored
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