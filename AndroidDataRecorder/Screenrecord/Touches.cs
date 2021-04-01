using System.Diagnostics;

namespace AndroidDataRecorder.Screenrecord
{
    public static class Touches
    {

        /// <summary>
        /// Method to get the current settings for show or hide screen touches
        /// </summary>
        /// <returns>returns true if setting is activated, false if deactivated </returns>
        public static bool GetStatus()
        {
            //create process to get touch settings
            var getStatus = new Process
            {
                StartInfo =
                {
                    //path to adb.exe
                    FileName = @"C:\Users\robin\OneDriveTHU\OneDrive - bwedu\Studium\platform-tools\adb.exe",
                    //add arguments for getting touch setting
                    Arguments = "exec-out settings get system show_touches",
                    //redirect standard input
                    RedirectStandardInput = true,
                    //redirect standard output
                    RedirectStandardOutput = true,
                    //use not shell execute
                    UseShellExecute = false
                }
            };

            //initialize variable for process input
            var line = "0";

            //start process
            getStatus.Start();
            
            //stay in loop while end of stream is not reached
            while (!getStatus.StandardOutput.EndOfStream)
            {
                //write line in variable
                line = getStatus.StandardOutput.ReadLine();
            }
            
            //close standard input of process
            getStatus.StandardInput.Close();

            //close process
            getStatus.Close();
            
            //convert 0/1 in boolean and return it
            return (line != "0");
        }
        
        /// <summary>
        /// create and start process with arguments to activate "show_touches" setting
        /// </summary>
        public static void ShowTouches()
        {
            //new process with settings to activate "show_touches" setting
            var showTouches = new Process
            {
                StartInfo =
                {
                    //path to adb.exe
                    FileName = @"C:\Users\robin\OneDriveTHU\OneDrive - bwedu\Studium\platform-tools\adb.exe",
                    //add arguments for showing touches
                    Arguments = "exec-out settings put system show_touches 1",
                    //redirect standard input
                    RedirectStandardInput = true,
                    //redirect standard output
                    RedirectStandardOutput = true,
                    //use not shell execute
                    UseShellExecute = false
                }
            };

            //start process
            showTouches.Start();
            
            //close standard input of process
            showTouches.StandardInput.Close();

            //close process
            showTouches.Close();
        }

        /// <summary>
        /// create and start process with arguments to deactivate "show_touches" setting
        /// </summary>
        public static void HideTouches()
        {
            //new process with settings to deactivate "show_touches" setting
            var hideTouches = new Process
            {
                StartInfo =
                {
                    //path to adb.exe
                    FileName = @"C:\Users\robin\OneDriveTHU\OneDrive - bwedu\Studium\platform-tools\adb.exe",
                    //add arguments for showing touches
                    Arguments = "exec-out settings put system show_touches 0",
                    //redirect standard input
                    RedirectStandardInput = true,
                    //redirect standard output
                    RedirectStandardOutput = true,
                    //use not shell execute
                    UseShellExecute = false
                }
            };

            //start process
            hideTouches.Start();
            
            //close standard input of process
            hideTouches.StandardInput.Close();

            //close process
            hideTouches.Close();
        }
    }
}