using System.Collections.ObjectModel;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Services
{
    public class LogBridge
    {
        public ObservableCollection<Filter> Filters = new ObservableCollection<Filter>();
    }
}