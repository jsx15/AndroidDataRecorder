# AndroidDataRecorder 

## Overview
This software is a data logger for android devices. It collects log data from one or more connected android devices and stores them into a database. Devices can either be connected via USB or network. For troubleshooting purposes, timestamps (markers) can be set, which allow the log data to be viewed in a self-defined period of time. The software also offers to record the screen of the connected devices and to create a video to a marker. Another function is the creation of issues. A backlog entry is created in JIRA, which contains selected markers, the associated log data for the selected time period, resources usage and, if desired, an MP4 video. So this software makes it easy for the user to find errors when testing android apps.
This software is designed to run on a Raspberry Pi or a computer of your choice.

## Prerequisities
Download the latest [Android SDK Platform-Tools](https://developer.android.com/studio/releases/platform-tools)  and [FFMPEG Version](https://ffmpeg.org/download.html).

To start the software it is necessary to edit the config.json file which is located in the “/src” directory.

**!! Please note necessary escape sequences !!**

The config.json file has the following parameters:

### Necessary:
<table>
  <thead>
    <th>Parameter Name</th>
    <th>Description</th>
    <th>Example</th>
  </thead>
  <tbody>
    <tr>
      <td>AdbPath</td>
      <td>the path to the platform-tools adb executablefile</td>
      <td>C:\\Program FIles\\platform-tools\\adb.exe</td>
    </tr>
    <tr>
      <td>FfmpegPath</td>
      <td>the path to the Ffmpeg executable</td>
      <td>C:\\Program FIles\\ffmpeg\\ffmpeg.exe</td>
    </tr>
    <tr>
      <td>VideoDirPath</td>
      <td>the path where the video files are stored (directory seperator appended)</td>
      <td>C:\\User\\Documents\</td>
    </tr>
    <tr>
      <td>TicketDirPath</td>
      <td>the path where the ticket files are stored</td>
      <td>C:\\User\\Documents\</td>
    </tr>
  </tbody>
</table>

### Optional:
<table>
  <thead>
    <th>Parameter Name</th>
    <th>Description</th>
    <th>Default Value</th>
  </thead>
  <tbody>
    <tr>
      <td>accessWorkloadInterval</td>
      <td>Interval the CPU and Memory is refreshed (1-60s)</td>
      <td>5 seconds</td>
    </tr>
    <tr>
      <td>JiraServerUrl</td>
      <td>URL to the JIRA Server</td>
      <td>-</td>
    </tr>
    <tr>
      <td>JiraUsername</td>
      <td>JIRA Username (e-mail)</td>
      <td>-</td>
    </tr>
    <tr>
      <td>ApiToken</td>
      <td>API Token from JIRA</td>
      <td>-</td>
    </tr>
  </tbody>
</table>
  
# Build

`git clone https://github.com/jsx15/AndroidDataRecorder.git`

`cd ./AndroidDataRecorder/AndroidDataRecorder/`

Build the software using dotnet: <br>
`dotnet build && dotnet run`

The software is accessible via https://localhost:5001.

## General Instructions

### Working on Raspberry Pi
The software can also be easily built and run on a Raspberry Pi. 

If the Raspberry Pi has a screen attached there is a special page designed for this screen. The page can be accessed via https://localhost:5001/pi.


## Contributors
* Konstantin Scholz (SCRUM Master)
* Robin Enderle
* Erwin Kenner
* Justin Reisch
* Jonas Schoeler 
* Sandra Schuart

## TERMS OF SERVICE

### Nugets
<table>
  <thead>
    <tr>
      <th>Title</th>
      <th>Description</th>
      <th>Author</th>
      <th>Published</th>
      <th>Version</th>
      <th>Link</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>Atlassian.SDK</td>
      <td>Utilities to interact with Atlassian products. Contains LinqToJira provider for querying JIRA Issue tracker (http://www.atlassian.com/software/jira)</td>
      <td>Federico Silva Armas</td>
      <td>January 19, 2021</td>
      <td>12.4.0</td>
      <td>https://www.nuget.org/packages/Atlassian.SDK/</td>
    </tr>
    <tr>
      <td>Blazored.Toast</td>
      <td>A JavaScript free Toast library for Blazor and Razor Components applications.</td>
      <td>Chris Sainty</td>
      <td>May 20, 2020</td>
      <td>3.1.2</td>
      <td>https://www.nuget.org/packages/Blazored.Toast</td>
    </tr>
     <tr>
      <td>grok.net</td>
      <td>Cross platform .NET grok implementation</td>
      <td>RMarusyk</td>
      <td>July 13, 2019</td>
      <td>1.0.1</td>
      <td>https://www.nuget.org/packages/grok.net</td>
    </tr>
     <tr>
      <td>Newtonsoft.Json</td>
      <td>Json.NET is a popular high-performance JSON framework for .NET</td>
      <td>James Newton-King</td>
      <td>March 22, 2021</td>
      <td>13.0.1</td>
      <td>https://www.nuget.org/packages/Newtonsoft.Json</td>
    </tr>
     <tr>
      <td>Plotly.Blazor</td>
      <td>Plotly.Blazor is a wrapper for plotly.js.
           Built on top of d3.js and stack.gl, plotly.js is a high-level, declarative charting library. It ships with over 40 chart types, including 3D charts, statistical graphs, and SVG maps.
           plotly.js is free and open source and you can view the source, report issues or contribute on GitHub.</td>
      <td>sean-laytec</td>
      <td>March 24, 2021</td>
      <td>2.0.0</td>
      <td>https://www.nuget.org/packages/Plotly.Blazor</td>
    </tr>
     <tr>
      <td>SharpAdbClient</td>
      <td>SharpAdbClient is a .NET library that allows .NET and .NET Core applications to communicate with Android devices.</td>
      <td>The Android Open Source Project, Ryan Conrad, Quamotion</td>
      <td>September 22, 2020</td>
      <td>2.3.23</td>
      <td>https://www.nuget.org/packages/SharpAdbClient</td>
    </tr>
     <tr>
      <td>System.Data.SQLite</td>
      <td>The official SQLite database engine for both x86 and x64 along with the ADO.NET provider.  This package includes support for LINQ and Entity Framework 6.</td>
      <td>SQLite Development Team</td>
      <td>December 24, 2020</td>
      <td>1.0.113.7</td>
      <td>https://www.nuget.org/packages/System.Data.SQLite</td>
    </tr>
  </tbody>
</table>
