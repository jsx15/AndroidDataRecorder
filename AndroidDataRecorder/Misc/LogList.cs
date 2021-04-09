using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Design;
using AndroidDataRecorder.Backend;

namespace AndroidDataRecorder.Misc
{
    public class LogList
    {
        public static List<LogEntry> _logs = new List<LogEntry>();
        
        private Database.Database data = new Database.Database();

        public LogList()
        {
            data.ConnectionToDatabase();
            
        }

        public void Update()
        {
        
        }
    }
}