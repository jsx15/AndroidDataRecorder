# Summary
This software is a data logger for android devices. It collects log data from one or more connected android devices and stores them into a database. Devices can either be connected via USB or network. For troubleshooting purposes, timestamps (markers) can be set which allow the log data to be viewed in a self-defined period of time. The software also offers to record the screen of the connected devices and to create a video to a marker. Another function is the creation of issues. A backlog entry is created in JIRA which contains selected markers, the associated log data for the selected time period, resources usage and, if desired, an MP4 video.

# Prerequisites
- [Dotnet 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) <br>
- [adb](https://developer.android.com/studio/releases/platform-tools) <br>
- [ffmpeg](https://ffmpeg.org/download.html)

# Build

`git clone https://github.com/jsx15/AndroidDataRecorder.git`

`cd ./AndroidDataRecorder/AndroidDataRecorder/`

Build the software using dotnet: <br>
`dotnet build && dotnet run`
- To bind the web server to an ip-address and port use parameter `--urls “http://192.168.100.2:5000”`. Replacing the address with * will bind to all interfaces.

# Configuration
To start the software it is necessary to edit the config.json file which is located in the “/src” directory.

**!! Please note necessary escape sequences !!**

The config.json file has the following parameters:<br>

##Necessary:<br>

AdbPath: the path to the platform-tools adb executable file                         (e.g. “C:\\Program FIles\\platform-tools\\adb.exe)

FfmpegPath: the path to the Ffmpeg executable

VideoDirPath: the path where the video files are stored (directory separator appended)

##Optional:

AccessWorkloadInterval: Interval the CPU and Memory is refreshed (1 - 60s). If nothing is entered it will be 5 seconds.

JiraServerUrl: URL to the JIRA Server


JiraUsername: JIRA Username (e-mail)


ApiToken: API Token from JIRA
