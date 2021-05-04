using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using AndroidDataRecorder.Misc;
using Atlassian.Jira;
using Newtonsoft.Json;

namespace AndroidDataRecorder.Backend
{
    public class TicketCreator
    {
        /// <summary>
        /// Jira RestClient
        /// </summary>
        private readonly Jira _jira;

        /// <summary>
        /// Url of the jira server
        /// </summary>
        private readonly String _jiraServerUrl = Config.GetJiraServerUrl();

        /// <summary>
        /// Username who connects to the jira server
        /// </summary>
        private readonly String _jiraUsername = Config.GetJiraUsername();

        /// <summary>
        /// API token for logging in
        /// </summary>
        private readonly String _apiToken = Config.GetApiToken();
        
        /// <summary>
        /// List of IssuePriorities
        /// </summary>
        public List<IssuePriority> PriorityList;

        /// <summary>
        /// List of ProjectKeys
        /// </summary>
        public List<string> KeyList;

        /// <summary>
        /// List of IssueTypes
        /// </summary>
        public List<IssueType> IssueTypeList;
        
        /// <summary>
        /// Supported output file formats
        /// </summary>
        public enum FileFormat
        {
            TextFile,
            JsonFile,
        }

        /// <summary>
        /// Constructor which should only be initialized once 
        /// </summary>
        public TicketCreator()
        {
            _jira = Jira.CreateRestClient(_jiraServerUrl, _jiraUsername, _apiToken);
            PriorityList = GetPriorities();
            IssueTypeList = GetIssueTypes();
            KeyList = GetProjectKeys();


            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"Tickets")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Tickets"));
            }
        }
        
        /// <summary>
        /// method to get ProjectKeys 
        /// </summary>
        /// <returns>returns List of Keys</returns>
        private List<string> GetProjectKeys()
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var token = tokenSource.Token;
            var tmp = _jira.Projects.GetProjectsAsync(token).Result;
            return tmp.Select(pro => pro.Key).ToList();
        }
        
        /// <summary>
        /// method to get ProjectPriorities
        /// </summary>
        /// <returns>returns List of IssuePriorities</returns>
        private List<IssuePriority> GetPriorities()
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var token = tokenSource.Token;
            var tmp = _jira.Priorities.GetPrioritiesAsync(token).Result;
            return tmp.OrderBy(x => x.Id).ToList();
        }

        /// <summary>
        /// method to get ProjectIssueTypes
        /// </summary>
        /// <returns>returns List of IssueTypes</returns>
        private List<IssueType> GetIssueTypes()
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var token = tokenSource.Token;
            var tmp = _jira.IssueTypes.GetIssueTypesAsync(token).Result;
            return tmp.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Creates a ticket with files attached
        /// </summary>
        /// <param name="combinedInfos">List of all selected markers</param>
        /// <param name="projectKey">Jira project key needed for jira</param>
        /// <param name="type">Ticket type</param>
        /// <param name="priority">Ticket priority</param>
        /// <param name="format">Output file format</param>
        /// <param name="summary">Summary of the created issue/ticket</param>
        /// <param name="description">Issue/ticket description</param>
        public void CreateTicket(List<Filter> combinedInfos, String projectKey, IssueType type,
            IssuePriority priority, FileFormat format, String summary, [Optional] String description)
        {
            var issue = _jira.CreateIssue(projectKey);
            issue.Type = type;
            issue.Priority = priority;
            issue.Summary = summary;
            issue.Description = description;
            issue.SaveChanges();

            String ticketDirPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
                @"Tickets" + Path.DirectorySeparatorChar + GetDate()));

            Directory.CreateDirectory(ticketDirPath);
            
            if (format == FileFormat.TextFile)
            {
                foreach (var info in combinedInfos)
                {
                    String fileName = Path.GetFullPath(Path.Combine(ticketDirPath, info.Marker.Devicename+ "_" +
                        info.Marker.MarkerId + info.Level + (info.timeSpanMinus + info.timeSpanPlus) + ".txt"));
                    CreateMarkerFile(info , fileName, projectKey); 
                    issue.AddAttachment(fileName);
                }
            } else if (format == FileFormat.JsonFile)
            {
                foreach (var info in combinedInfos)
                {
                    String fileName = Path.GetFullPath(Path.Combine(ticketDirPath, info.Marker.Devicename + "_" 
                        + info.Marker.MarkerId + info.Level + (info.timeSpanMinus + info.timeSpanPlus) + ".json"));
                    CreateMarkerJson(info , fileName); 
                    issue.AddAttachment(fileName);
                }
            }

            foreach (var filename in from filter in combinedInfos where filter.CreateVideo select Config.GetVideoDirPath + "marker_" + filter.Marker.MarkerId + "_" +
                filter.Marker.Devicename + ".mp4")
            {
                issue.AddAttachment(filename);
            }
        }
        
        /// <summary>
        /// private methode used in CreateTicketTxt, which creates files that will be attached
        /// </summary>
        /// <param name="info">combinedInfo object</param>
        /// <param name="file">File path</param>
        /// <param name="projectKey">Project Key</param>
        private void CreateMarkerFile(Filter info, String file, string projectKey)
        {
            FileInfo fi = new FileInfo(file);
            
            /*
            List<TicketEntry> list = new List<TicketEntry>();
            info.Logs.ForEach(entry => list.Add(new TicketEntry(entry.timeStamp, entry.ToString())));
            info.Resources.ForEach(resourcesList => list.Add(new TicketEntry(resourcesList.timestamp,
                "##" + key + "##" + resourcesList)));
            list.Add(new TicketEntry(info.marker.timeStamp,"###"+ key + "###" + info.marker));
            */
            
            List<TicketEntry> list = new List<TicketEntry>();
            
            info.Logs.ForEach(entry => list.Add(new TicketEntry(entry.TimeStamp, entry.Devicename,
                entry.DeviceSerial,entry.DeviceTimestamp, entry.Pid, entry.Tid, entry.LogLevel, entry.App, entry.Message)));
            
            info.Resources.ForEach(resourcesList => list.Add(new TicketEntry(resourcesList.Timestamp,
                resourcesList.DeviceName, resourcesList.Serial, resourcesList.Cpu, resourcesList.Memory, resourcesList.Battery)));
            
            list.Add(new TicketEntry(info.Marker.TimeStamp, info.Marker.Devicename, info.Marker.DeviceSerial,
                info.Marker.Message, info.Marker.MarkerId));
            
            list.Sort((x,y)=> DateTime.Compare(x.timestamp , y.timestamp));
            
            try    
            {    
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("####");

                    sw.WriteLine("#Logs from Device: {0}", info.Marker.Devicename);
                    sw.WriteLine("#at {0} minutes before and {1} after Marker with ID: {2}",
                        info.timeSpanMinus, info.timeSpanPlus, info.Marker.MarkerId);
                    sw.WriteLine("#Marker message: {0}", info.Marker.Message);
                    sw.WriteLine("####");

                    foreach (var entry in list )
                    {
                        sw.WriteLine(entry.ToString(projectKey));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// private methode used in CreateTicketJson, which creates files that will be attached
        /// </summary>
        /// <param name="info">combinedInfo object</param>
        /// <param name="file">File path</param>
        private void CreateMarkerJson(Filter info, String file)
        {
            FileInfo fi = new FileInfo(file);

            List<TicketEntry> list = new List<TicketEntry>();
            
            info.Logs.ForEach(entry => list.Add(new TicketEntry(entry.TimeStamp, entry.Devicename,
                entry.DeviceSerial,entry.DeviceTimestamp, entry.Pid, entry.Tid, entry.LogLevel, entry.App, entry.Message)));
            
            info.Resources.ForEach(resourcesList => list.Add(new TicketEntry(resourcesList.Timestamp,
                resourcesList.DeviceName, resourcesList.Serial, resourcesList.Cpu, resourcesList.Memory, resourcesList.Battery)));
            
            list.Add(new TicketEntry(info.Marker.TimeStamp, info.Marker.Devicename, info.Marker.DeviceSerial,
                info.Marker.Message, info.Marker.MarkerId));
            
            list.Sort((x,y)=> DateTime.Compare(x.timestamp , y.timestamp));

            //List<string> entries = new List<string>();
            //list.ForEach(entry => entries.Add(entry.ToString()));
            string json = JsonConvert.SerializeObject(list.ToArray());
            
            try
            {
                if (fi.Exists)
                {
                    fi.Delete();
                }
                
                File.WriteAllText(file, json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Used to replace spaces and slashes from DateTime.Now.ToString() with underscores
        /// </summary>
        /// <returns>alternative string value for DateTime.Now.ToString()</returns>
        private string GetDate()
        { 
            return DateTime.Now.ToString("MM-dd-yy_HH-mm-ss");
        }
        
        /// <summary>
        /// helper class 
        /// </summary>
        private class TicketEntry
        {
            public DateTime timestamp;
            public string DeviceName;
            public string DeviceSerial;
            
            public string Message;

            public string DeviceTimestamp;
            public string Pid;
            public string Tid;
            public string Loglevel;
            public string App;

            public string Cpu;
            public string Memory;
            public string Battery;

            public string MarkerId;

            /// <summary>
            /// Constructor for Logs
            /// </summary>
            /// <param name="timestamp">timestamp</param>
            /// <param name="deviceName">deviceName</param>
            /// <param name="deviceSerial">deviceSerial</param>
            /// <param name="deviceTimestamp">deviceTimestamp</param>
            /// <param name="pid">Pid</param>
            /// <param name="tid">Tid</param>
            /// <param name="loglevel">Loglevel</param>
            /// <param name="app">App</param>
            /// <param name="message">Message</param>
            public TicketEntry(DateTime timestamp, string deviceName, string deviceSerial, DateTime deviceTimestamp,
                int pid, int tid, string loglevel, string app, string message)
            {
                this.timestamp = timestamp;
                Message = message;
                DeviceName = deviceName;
                DeviceSerial = deviceSerial;
                DeviceTimestamp = deviceTimestamp.ToString(CultureInfo.InvariantCulture);
                Pid = pid.ToString();
                Tid = tid.ToString();
                Loglevel = loglevel;
                App = app;
            }
            
            /// <summary>
            /// Constructor for Resource
            /// </summary>
            /// <param name="timestamp">Timestamp</param>
            /// <param name="deviceName">DeviceName</param>
            /// <param name="deviceSerial">DeviceSerial</param>
            /// <param name="cpu">Cpu</param>
            /// <param name="memory">Memory</param>
            /// <param name="battery">Battery</param>
            public TicketEntry(DateTime timestamp, string deviceName, string deviceSerial, int cpu,
                int memory, int battery)
            {
                this.timestamp = timestamp;
                DeviceName = deviceName;
                DeviceSerial = deviceSerial;
                Cpu = cpu.ToString();
                Memory = memory.ToString();
                Battery = battery.ToString();
            }
            
            /// <summary>
            /// Constructor for Marker
            /// </summary>
            /// <param name="timestamp">Timestamp</param>
            /// <param name="deviceName">DeviceName</param>
            /// <param name="deviceSerial">DeviceSerial</param>
            /// <param name="message">Message</param>
            /// <param name="markerId">MarkerId</param>
            public TicketEntry(DateTime timestamp, string deviceName, string deviceSerial, string message, int markerId)
            {
                this.timestamp = timestamp;
                Message = message;
                DeviceName = deviceName;
                DeviceSerial = deviceSerial;
                MarkerId = markerId.ToString();
            }

            /// <summary>
            /// overrides ToString()
            /// </summary>
            /// <returns>string for line in TextFile</returns>
            public string ToString(string key)
            {
                if (DeviceTimestamp != null)
                {
                    return timestamp +" "+ DeviceSerial +" "+ DeviceName + " "  + DeviceTimestamp + " " + Pid + " " +
                           Tid + " " + Loglevel + " " + App + ": " + Message;
                }
                
                if (Cpu != null)
                {
                    return "##" + key + "## " + timestamp + "Device serial: " + DeviceSerial + " Device name: " + DeviceName + " CPU load: " +
                           Cpu + " Memory usage: " + Memory + " Battery status: " + Battery;
                }
                
                if (MarkerId != null)
                {
                    return "###" + key + "### " + timestamp +" MarkerID:  " + MarkerId + "  MarkerMessage:  " + Message;
                }
                
                return timestamp + " " + DeviceSerial + " " + DeviceName;

            }
        }
    }
}