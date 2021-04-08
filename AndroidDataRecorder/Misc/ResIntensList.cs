namespace AndroidDataRecorder.Misc
{
    public class ResIntensList
    {
        public int cpu { get; set; }
        public int memory { get; set; }
        public string app { get; set; }
        
        public ResIntensList(){}

        public ResIntensList(int _cpu, int _memory, string _app)
        {
            cpu = _cpu;
            memory = _memory;
            app = _app;
        }
    }
}