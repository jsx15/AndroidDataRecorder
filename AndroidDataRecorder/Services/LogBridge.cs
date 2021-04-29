using System.Collections.Generic;
using System.Collections.ObjectModel;
using AndroidDataRecorder.Misc;
using AndroidDataRecorder.Pages;

namespace AndroidDataRecorder.Services
{
    public class LogBridge
    {
        public ObservableCollection<Filter> Filters = new ObservableCollection<Filter>();
    }
}