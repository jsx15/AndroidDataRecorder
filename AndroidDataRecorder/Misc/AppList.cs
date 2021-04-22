using System;

namespace AndroidDataRecorder.Misc
{
    public class AppList
    {
        
        public string appName { get; set; }
        
        public string serialFK { get; set; }
        
        public AppList(){}

        public AppList(string _appName, string _serialFk)
        {
            appName = _appName;
            serialFK = _serialFk;
        }
        
    }
}