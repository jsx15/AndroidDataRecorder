using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using AndroidDataRecorder.Misc;
using Atlassian.Jira;

namespace AndroidDataRecorder.Backend
{
    public class TicketCreator
    {
        private readonly Jira _jira;

        private String _jiraServerUrl;

        private String _jiraUsername;

        private String _apiToken;
        
        public enum TicketType
        {
            Bug,
            Story,
            Task,
        }
        
        public enum TicketPriority
        {
            Highest,
            High,
            Medium,
            Low,
            Lowest,
        }

        public TicketCreator()
        {
            _jiraServerUrl = "https://androiddebugger.atlassian.net/";
            _jiraUsername = "e.kenner@gmx.net";
            _apiToken = "XfgTiD10CbH1g0EvZkaw4974";
            _jira = Jira.CreateRestClient(_jiraServerUrl, _jiraUsername, _apiToken);
        }

        public void CreateTicketTxt(List<Demo.combinedInfo> combinedInfos, String projectKey, TicketType type,
            TicketPriority priority, String summary, String assignee, [Optional] String description)
        {
            var issue = _jira.CreateIssue(projectKey);
            issue.Type = type.ToString();
            issue.Priority = priority.ToString();
            issue.Summary = summary;
            issue.Assignee = assignee;
            issue.Description = description;
            issue.SaveChanges();
            String ticketDirPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
                @"Tickets" + Path.DirectorySeparatorChar + GetDate()));

            Directory.CreateDirectory(ticketDirPath);
            
            foreach (var info in combinedInfos)
            {
                String fileName = Path.GetFullPath(Path.Combine(ticketDirPath, info._marker.devicename + "_" +
                                                             info._marker.markerId + ".txt"));
                CreateMarkerFile(info , fileName);
                issue.AddAttachment(fileName);
            }
            
            
        }
        
        public void CreateTicketJson(List<Demo.combinedInfo> combinedInfos, String projectKey, TicketType type,
            TicketPriority priority, String summary, String assignee, [Optional] String description)
        {
            var issue = _jira.CreateIssue(projectKey);
            issue.Type = type.ToString();
            issue.Priority = priority.ToString();
            issue.Summary = summary;
            issue.Assignee = assignee;
            issue.Description = description;
            issue.SaveChanges();
            String ticketDirPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
                @"Tickets" + Path.DirectorySeparatorChar + GetDate()));

            Directory.CreateDirectory(ticketDirPath);
            
            foreach (var info in combinedInfos)
            {
                String fileName = Path.GetFullPath(Path.Combine(ticketDirPath, info._marker.devicename + "_" +
                                                                               info._marker.markerId + ".json"));
                CreateMarkerJson(info , fileName);
                issue.AddAttachment(fileName);
            }
            
            
        }

        private void CreateMarkerFile(Demo.combinedInfo info, String file)
        {
            FileInfo fi = new FileInfo(file);

            LogEntry markerLogEntry = new LogEntry(info._marker.devicename, info._marker.timeStamp,
                info._marker.timeStamp, -1, -1, null, null, info._marker.message);
            
            info._logs.Add(markerLogEntry);
            info._logs.Sort((x, y) => DateTime.Compare(x.timeStamp, y.timeStamp));
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

                    foreach (var log in info._logs)
                    {
                        sw.WriteLine(log.ToString());
                    }
                }
            }    
            catch (Exception ex)    
            {    
                Console.WriteLine(ex.ToString());    
            }
        }

        private void CreateMarkerJson(Demo.combinedInfo info, String file)
        {
            FileInfo fi = new FileInfo(file);
            
            LogEntry markerLogEntry = new LogEntry(info._marker.devicename, info._marker.timeStamp,
                info._marker.timeStamp, -1, -1, null, null, info._marker.message);

            info._logs.Add(markerLogEntry);
            info._logs.Sort((x, y) => DateTime.Compare(x.timeStamp, y.timeStamp));

            String json = JsonSerializer.Serialize(info._logs);

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

        private string GetDate()
        {
            String temp = DateTime.Now.ToString().Replace("/","_");
            

            return temp.Replace(" ","_");
        }


    }
}