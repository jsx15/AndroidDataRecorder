using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using AndroidDataRecorder.Misc;
using Atlassian.Jira;

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
        /// Assignee field for Issue determined by username
        /// </summary>
        private readonly string _assignee;
        
        /// <summary>
        /// Supported issue/ticket types
        /// </summary>
        public enum TicketType
        {
            Bug,
            Story,
            Task,
        }
        
        /// <summary>
        /// Supported issue/ticket types
        /// </summary>
        public enum TicketPriority
        {
            Highest,
            High,
            Medium,
            Low,
            Lowest,
        }
        
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
            _assignee = _jira.Users.GetMyselfAsync().Result.DisplayName;
            
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"Tickets")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Tickets"));
            }
        }
        
        /// <summary>
        /// Get a List of all projects
        /// </summary>
        /// <returns>List with all projects</returns>
        public List<Project> GetMyProjects()
        {
            return _jira.Projects.GetProjectsAsync().Result.ToList();
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
        public void CreateTicket(List<Demo.combinedInfo> combinedInfos, String projectKey, TicketType type,
            TicketPriority priority,FileFormat format, String summary, [Optional] String description)
        {
            var issue = _jira.CreateIssue(projectKey);
            issue.Type = type.ToString();
            issue.Priority = priority.ToString();
            issue.Summary = summary;
            issue.Assignee = _assignee;
            issue.Description = description;
            issue.SaveChanges();
            String ticketDirPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
                @"Tickets" + Path.DirectorySeparatorChar + GetDate()));

            Directory.CreateDirectory(ticketDirPath);
            
            if (format == FileFormat.TextFile)
            {
                foreach (var info in combinedInfos)
                {
                    String fileName = Path.GetFullPath(Path.Combine(ticketDirPath, info._marker.devicename + "_" 
                        + info._marker.markerId + ".txt"));
                    CreateMarkerFile(info , fileName); 
                    issue.AddAttachment(fileName);
                }
            } else if (format == FileFormat.JsonFile)
            {
                foreach (var info in combinedInfos)
                {
                    String fileName = Path.GetFullPath(Path.Combine(ticketDirPath, info._marker.devicename + "_" 
                        + info._marker.markerId + ".json"));
                    CreateMarkerJson(info , fileName); 
                    issue.AddAttachment(fileName);
                }
            }
        }
        
        /// <summary>
        /// private methode used in CreateTicketTxt, which creates files that will be attached
        /// </summary>
        /// <param name="info">combinedInfo object</param>
        /// <param name="file">File path</param>
        private void CreateMarkerFile(Demo.combinedInfo info, String file)
        {
            FileInfo fi = new FileInfo(file);

            List<TicketEntry> list = new List<TicketEntry>();
            info._logs.ForEach(entry => list.Add(new TicketEntry(entry.timeStamp, entry.ToString())));
            info.Resources.ForEach(resourcesList => list.Add(new TicketEntry(resourcesList.timestamp, resourcesList.ToString())));
            list.Add(new TicketEntry(info._marker.timeStamp,info._marker.ToString()));
            
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
                    sw.WriteLine("#Logs from Device: {0}", info._marker.devicename);
                    sw.WriteLine("#at {0} minutes before and {1} after Marker with ID: {2}",
                        info.timeSpanMinus,info.timeSpanPlus, info._marker.markerId);
                    sw.WriteLine("#Marker message: {0}", info._marker.message);
                    sw.WriteLine("####");

                    foreach (var entry in list )
                    {
                        sw.WriteLine(entry.lineText);
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
        private void CreateMarkerJson(Demo.combinedInfo info, String file)
        {
            FileInfo fi = new FileInfo(file);
            
            //LogEntry markerLogEntry = new LogEntry(info._marker.serial, info._marker.devicename, info._marker.timeStamp,
            // info._marker.timeStamp, -1, -1, null, null, info._marker.message);

            List<TicketEntry> list = new List<TicketEntry>();
            info._logs.ForEach(entry => list.Add(new TicketEntry(entry.timeStamp, entry.ToString())));
            info.Resources.ForEach(resourcesList => list.Add(new TicketEntry(resourcesList.timestamp, resourcesList.ToString())));
            list.Add(new TicketEntry(info._marker.timeStamp,info._marker.ToString()));
            
            list.Sort((x,y)=> DateTime.Compare(x.timestamp , y.timestamp));

            //info._logs.Add(markerLogEntry);
            //info._logs.Sort((x, y) => DateTime.Compare(x.timeStamp, y.timeStamp));

            String json = JsonSerializer.Serialize(list);

            try
            {
                if (fi.Exists) 
                {
                    fi.Delete(); 
                }
                File.WriteAllText(file,json);
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
            String temp = DateTime.Now.ToString().Replace("/","_");
            
            return temp.Replace(" ","_");
        }
        
        private class TicketEntry
        {
            public String lineText;
            public DateTime timestamp;
            public TicketEntry(DateTime timestamp, String lineText)
            {
                this.timestamp = timestamp;
                this.lineText = lineText;
            }
        }
    }
}