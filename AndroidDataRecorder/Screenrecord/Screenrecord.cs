using System;
using System.Collections.Generic;
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

        //a list with files in use
        public static readonly List<string> FilesInUse = new List<string>();

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
        /// start thread for screen record
        /// </summary>
        public void StartScreenrecord()
        {
            //variable for device ID
            var deviceSerial = _deviceObj.Serial;

            //set record state to true
            _record = true;

            //create thread
            var recordProcess = new Thread(() => PrepareRecord(deviceSerial));

            //start thread
            recordProcess.Start();
        }

        /// <summary>
        /// prepare process for screen record
        /// create path for files
        /// check for maximum video number -> delete file
        /// start merging files
        /// delete all files to get one video
        /// </summary>
        /// <param name="deviceSerial">device id</param>
        private void PrepareRecord(string deviceSerial)
        {
            //prepare adb process
            var scProc = new Process
            {
                StartInfo =
                {
                    //path to adb.exe
                    FileName = Config.GetAdbPath(),
                    //add arguments for screen record
                    Arguments = "-s " + deviceSerial + " exec-out screenrecord --output-format=h264 - ",
                    //redirect standard input
                    RedirectStandardInput = true,
                    //redirect standard output
                    RedirectStandardOutput = true,
                    //use not shell execute
                    UseShellExecute = false
                }
            };

            //create path for files and set local path variable
            var path = VideoPath.Create(deviceSerial);

            //safe touch settings for restore
            var touchState = Touches.GetStatus(deviceSerial);

            //check if touch already active
            if (!touchState)
            {
                //show screen touch
                Touches.ShowTouches(deviceSerial);
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
                Touches.HideTouches(deviceSerial);
            }

            Console.WriteLine("Record process finished " + _deviceObj.Name);
        }

        /// <summary>
        /// start and stop the screen record process
        /// create video file
        /// store the buffered input in the video file
        /// create stopwatch to handle record length
        /// </summary>
        /// <param name="path">path to store the video files</param>
        /// <param name="scProc">adb screen record process</param>
        private void HandleScreenrecord(string path, Process scProc)
        {
            //get timestamp 
            var time = Timestamp.GetTimestamp();

            //create path with timestamp for a file
            var file = path + time + ".mp4";

            //check if path is null and stop if it is null
            if (path == null) return;

            //add file to files in use list
            FilesInUse.Add(file);

            Console.WriteLine("Start recording " + _deviceObj.Name + "   " + file);

            //create byte buffer
            var buffer = new byte[50];

            //create file for filestream
            var output = File.Open(file, FileMode.Create);

            //start screen record process
            scProc.Start();

            //create a buffered stream with the standard output of the screen record process
            var input = new BufferedStream(scProc.StandardOutput.BaseStream);

            //Create a new Thread to read bytes
            var thread = new Thread((() => Recording(input, buffer, output, file)));

            //start thread
            thread.Start();

            //create stopwatch for timeout recognition
            var waitForVideo = new Stopwatch();

            //start stopwatch
            waitForVideo.Start();

            //wait for timeout or recording is finished
            while (waitForVideo.ElapsedMilliseconds < _videoLength + 2000 && thread.IsAlive)
            {
            }

            //kill process before closing streams
            scProc.Kill();

            //close output stream
            output.Close();

            //close input stream
            input.Close();

            Console.WriteLine("Recording stopped " + _deviceObj.Name + "   " + Timestamp.GetTimestamp());
        }

        /// <summary>
        /// Method to read and write bytes from screen recorder
        /// </summary>
        /// <param name="input">input stream of screen record</param>
        /// <param name="buffer">byte array buffer</param>
        /// <param name="output">output file stream</param>
        /// <param name="path"></param>
        void Recording(Stream input, byte[] buffer, Stream output, string path)
        {
            //create Stopwatch
            var sw = new Stopwatch();

            //start stopwatch
            sw.Start();
            
            do
            {
                //read the input buffer of the screen record process
                var len = input.Read(buffer, 0, buffer.Length);

                try
                {
                    //write buffer in video file
                    output.Write(buffer, 0, len);
                }
                // catch exception when output is already closed -> closed because timeout
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("Output already closed -> Timeout " + Timestamp.GetTimestamp());
                    
                    //delete file -> device was in standby -> file not readable
                    File.Delete(path);
                }

                //repeat if video length is not reached or record should be stopped 
            } while (sw.ElapsedMilliseconds < _videoLength && _record);

            //remove file out of list
            FilesInUse.Remove(path);
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