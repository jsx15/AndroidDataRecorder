using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
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
            try
            {
                issue.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                throw new IssueTypeNotSupported();
            }

            String ticketDirPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
                @"Tickets" + Path.DirectorySeparatorChar + GetDate()));

            Directory.CreateDirectory(ticketDirPath);
            
            if (format == FileFormat.TextFile)
            {
                foreach (var info in combinedInfos)
                {
                    String fileName = Path.GetFullPath(Path.Combine(ticketDirPath, info.marker.devicename+ "_" +
                        info.marker.MarkerId + info.Level + (info.timeSpanMinus + info.timeSpanPlus) + ".txt"));
                    CreateMarkerFile(info , fileName); 
                    issue.AddAttachment(fileName);
                }
            } else if (format == FileFormat.JsonFile)
            {
                foreach (var info in combinedInfos)
                {
                    String fileName = Path.GetFullPath(Path.Combine(ticketDirPath, info.marker.devicename + "_" 
                        + info.marker.MarkerId + info.Level + (info.timeSpanMinus + info.timeSpanPlus) + ".json"));
                    CreateMarkerJson(info , fileName); 
                    issue.AddAttachment(fileName);
                }
            }

            foreach (var filename in from filter in combinedInfos where filter.CreateVideo select Config.GetVideoDirPath + "marker_" + filter.marker.MarkerId + "_" +
                filter.marker.devicename + ".mp4")
            {
                issue.AddAttachment(filename);
            }
        }
        
        /// <summary>
        /// private methode used in CreateTicketTxt, which creates files that will be attached
        /// </summary>
        /// <param name="info">combinedInfo object</param>
        /// <param name="file">File path</param>
        private void CreateMarkerFile(Filter info, String file)
        {
            FileInfo fi = new FileInfo(file);

            List<TicketEntry> list = new List<TicketEntry>();
            info.Logs.ForEach(entry => list.Add(new TicketEntry(entry.timeStamp, entry.ToString())));
            info.Resources.ForEach(resourcesList => list.Add(new TicketEntry(resourcesList.timestamp, resourcesList.ToString())));
            list.Add(new TicketEntry(info.marker.timeStamp,info.marker.ToString()));
            
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

                    sw.WriteLine("#Logs from Device: {0}", info.marker.devicename);
                    sw.WriteLine("#at {0} minutes before and {1} after Marker with ID: {2}",
                        info.timeSpanMinus, info.timeSpanPlus, info.marker.MarkerId);
                    sw.WriteLine("#Marker message: {0}", info.marker.message);
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
        private void CreateMarkerJson(Filter info, String file)
        {
            FileInfo fi = new FileInfo(file);

            List<TicketEntry> list = new List<TicketEntry>();
            info.Logs.ForEach(entry => list.Add(new TicketEntry(entry.timeStamp, entry.ToString())));
            info.Resources.ForEach(resourcesList => list.Add(new TicketEntry(resourcesList.timestamp, resourcesList.ToString())));
            list.Add(new TicketEntry(info.marker.timeStamp,info.marker.ToString()));
            
            list.Sort((x,y)=> DateTime.Compare(x.timestamp , y.timestamp));
            
            String json = JsonSerializer.Serialize(list);

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