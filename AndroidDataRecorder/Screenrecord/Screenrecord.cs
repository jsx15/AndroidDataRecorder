using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using AndroidDataRecorder.Backend;
using SharpAdbClient;

namespace AndroidDataRecorder.Screenrecord
{
    public class Screenrecord
    {
        //length of video snippets
        private readonly int _videoLength;

        //bool whether the recording should run or not 
        private volatile bool _record;

        //connected device
        private readonly DeviceData _deviceObj;

        /// <summary>
        /// constructor to initialize:
        /// </summary>
        /// <param name="deviceObj">connected device</param>
        /// <param name="videoLength">set video length</param>
        public Screenrecord(DeviceData deviceObj, int videoLength)
        {
            //set video length
            this._videoLength = videoLength;

            //set device object
            this._deviceObj = deviceObj;
        }

        /// <summary>
        /// start thread for screenrecord
        /// </summary>
        public void StartScreenrecord()
        {
            //variable for device ID
            string deviceId = _deviceObj.ToString();

            //set record state to true
            _record = true;

            //create thread
            var recordProcess = new Thread(() => PrepareRecord(deviceId));

            //start thread
            recordProcess.Start();
        }

        /// <summary>
        /// prepare process for screenrecord
        /// create path for files
        /// check for maximum video number -> delete file
        /// start merging files
        /// delete all files to get one video
        /// </summary>
        /// <param name="device">device id</param>
        private void PrepareRecord(string device)
        {
            //prepare adb process
            var scProc = new Process
            {
                StartInfo =
                {
                    //path to adb.exe
                    FileName = Config.GetAdbPath(),
                    //add arguments for screenrecord
                    Arguments = "-s " + device + " exec-out screenrecord --output-format=h264 - ",
                    //redirect standard input
                    RedirectStandardInput = true,
                    //redirect standard output
                    RedirectStandardOutput = true,
                    //use not shell execute
                    UseShellExecute = false
                }
            };

            //create path for files and set local path variable
            var path = VideoPath.Create(device);

            //safe touch settings for restore
            var touchState = Touches.GetStatus(device);

            //check if touch already active
            if (!touchState)
            {
                //show screen touch
                Touches.ShowTouches(device);
            }

            //record while bool is true
            while (_record)
            {
                //start recording
                HandleScreenrecord(path, scProc);
            }

            //close standard input of process
            scProc.StandardInput.Close();

            //close process
            scProc.Close();

            //check if touch setting was off
            if (!touchState)
            {
                //hide screen touch
                Touches.HideTouches(device);
            }
            Console.WriteLine("Record process finished " + _deviceObj.Name);
        }

        /// <summary>
        /// start and stop the screenrecord process
        /// create video file
        /// store the buffered input in the video file
        /// create stopwatch to handle record length
        /// </summary>
        /// <param name="path">path to store the video files</param>
        /// <param name="scProc">adb screenrecord process</param>
        private void HandleScreenrecord(string path, Process scProc)
        {
            //get timestamp 
            var time = Timestamp.GetTimestamp();

            //create path with timestamp for a file
            var file = path + time + ".mp4";

            //check if path is null and stop if it is null
            if (path == null) return;

            Console.WriteLine("Start recording " + _deviceObj.Name);

            //create byte buffer
            var buffer = new byte[1024];

            //create file for filestream
            var output = File.Open(file, FileMode.Create);

            //create Stopwatch
            var sw = new Stopwatch();

            //start stopwatch
            sw.Start();

            //start screenrecord process
            scProc.Start();

            //create a buffered stream with the standard output of the screenrecord process
            var input = new BufferedStream(scProc.StandardOutput.BaseStream);

            //store data in file while 
            do
            {
                //read the input buffer of the screenrecord process
                var len = input.Read(buffer, 0, buffer.Length);

                //write buffer in video file
                output.Write(buffer, 0, len);

                //repeat if video length is not reached or record should be stopped 
            } while (sw.ElapsedMilliseconds < _videoLength && _record);

            //kill process before closing streams
            scProc.Kill();

            //close output stream
            output.Close();

            //close input stream
            input.Close();

            Console.WriteLine("Recording stopped " + _deviceObj.Name);
        }

        /// <summary>
        /// method to set record variable to false
        /// </summary>
        public void StopRecording()
        {
            //set variable to false
            _record = false;
            
            Console.WriteLine("stop recording marked " + _deviceObj.Name);
        }
    }
}