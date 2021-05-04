
namespace AndroidDataRecorder.Misc
{
    public class AppList
    {
        
        public string AppName { get; set; }
        
        public string SerialFk { get; set; }
        
        public AppList(){}

        public AppList(string appName, string serialFk)
        {
            AppName = appName;
            SerialFk = serialFk;
        }
        
    }
}