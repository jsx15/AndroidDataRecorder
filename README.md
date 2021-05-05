# AndroidDataRecorder 

# Overview
This software is a data logger for android devices. It collects log data from one or more connected android devices and stores them into a database. Devices can either be connected via USB or network. For troubleshooting purposes, timestamps (markers) can be set, which allow the log data to be viewed in a self-defined period of time. The software also offers to record the screen of the connected devices and to create a video to a marker. Another function is the creation of issues. A backlog entry is created in JIRA, which contains selected markers, the associated log data for the selected time period, resources usage and, if desired, an MP4 video. So this software makes it easy for the user to find errors when testing android apps.
This software is designed to run on a Raspberry Pi or a computer of your choice.

## Contributors
* Konstantin Scholz (SCRUM Master)
* Robin Enderle
* Erwin Kenner
* Justin Reisch
* Jonas Schoeler 
* Sandra Schuart

# Prerequisities
Download the latest [Android SDK Platform-Tools](https://developer.android.com/studio/releases/platform-tools)  and [FFMPEG Version](https://ffmpeg.org/download.html).

To start the software it is necessary to edit the config.json file which is located in the “/src” directory.

**!! Please note necessary escape sequences !!**

The config.json file has the following parameters:

## Necessary:
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
  </tbody>
</table>

## Optional:
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

# General Instructions

## Working on Raspberry Pi
The software can also be easily built and run on a Raspberry Pi. 

If the Raspberry Pi has a screen attached there is a special page designed for this screen. The page can be accessed via https://localhost:5001/pi.

![pi-page](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/pipage.PNG)

**Note:** When Stop/Start Logging or Start/Stop Recording is pressed logging or recording is started or stopped for every device attached. Markers are also set for every device attached.
The CPU and Memory usage for a current selected device can also be seen on that page. Change the device by clicking on the line chart.

## Working on a Computer

Browse to https://localhost:5001/ to open the web-app.

### Dashboard
The upper-left side of the dashboard shows an overview about the currently connected devices. Each device connected to the host is listed here and logging is automatically started for each device.

<ul>
  <li>Logging can be stopped and started separately for every device by clicking the “Start/Stop Logging” button</li>
  <li>Screen Recording can be stopped and started separately for every device by clicking the “Start/Stop Recording” button</li>
  <li>A device, that is connected via wifi, can be disconnected by clicking the Disconnect button</li>
</ul>

To select a device, which is necessary for setting markers and displaying resources usage, a simple click on the device name is needed and the selection will be active. On the bottom of the card the selected device can be seen.

![overview](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/overviewcard.PNG)

In the upper-right side markers can be placed. This is achieved by simply entering a description and pressing the **Add Marker button**. The current time and date are shown below the input field. By clicking **Show All Markers** the user is forwarded to the Marker page (for further instructions see Marker).

![marker](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/marker.PNG)

The **System Stats** section shows a chart on the left and table with 5 rows on the right.
The chart shows the CPU and Memory usage at a range of 0% - 100% of the selected device with a history of 15 timestamps. By hovering over the points the exact usage is shown.

The table displays the current 5 most expensive processes on the selected device with their cpu and memory usage. The range is the same as in the chart.
Both, the chart and the table are refreshed either configured in the config.json file or by default 5 seconds.

![marker](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/systemstats.PNG)

### Marker

On the **Marker** page every marker is listed for the selected device. The device can be changed by clicking the dropdown in the upper-left.
As can be seen in the picture every marker appears with its ID, timestamp and message. By clicking the red cross at the right the marker is deleted. It is not possible to restore the deleted marker.
Markers can be selected by simply clicking on the row, which then turns blue as a hint for selection.
After finishing the selection of all markers needed for further steps, the **Apply** button is forwarding to the **Logger** page.

![markerpage](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/Markerpage.PNG)

### Logger

On this page, the log data associated with the markers can be filtered and displayed according to a time period and the log level. Each marker is handled individually, which makes it possible to get only the requested log data that is needed at that marker. It is also possible to use a marker twice with different settings.
All markers selected on the **Marker** page appear automatically on the **Logger** page. By clicking **Add Filter** more filters can be added. 
By pressing **Apply filter**, the filter is applied and the associated log data is displayed in a table below the filter settings. **Add ticket** adds the marker with its filter selection to the ticket page (see Ticket). To switch to the Ticket page simply press **Go to ticket creation**.

The filtering system is quite easy:
<ul>
  <li>Marker: Shows the selected marker with its message</li>
  <li>Shows the device associated to the marker</li>
  <li>Timespan - (Minutes): The time in minutes before the marker</li>
  <li>Timespan + (Minutes): The time in minutes after the marker</li>
  <li>Loglevel: The loglevel of the logs (V, D, I, W, E, F)</li>
</ul>

**Note:** When a loglevel is selected, the higher priority log levels are always included in the output. F has the highest priority, V the lowest. This means that if log level E is selected, all logs with log levels E and F are also outputted. To get all logs of all levels V must be selected.

An example can be seen in the picture below:
Our system crashed, a marker was set and selected. This marker now appears on the logger page. Since it is not 100% certain, when the crash happened, we now want to see the log data in the time span 0.5 minutes before and 0.5 minutes after. Since we are dealing with a complete crash of the application, we only want to see the log data of level E and F, so we select E as the log level. After applying the filter, all log data recorded at this time will appear in the table.

![filterpage](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/filterpage.PNG)

### Ticket

This page is used to create a bug ticket from the previously filtered log data. It is possible to create a JIRA backlog entry. (see Prerequisites)
All filtered markers with their log data previously added to a ticket appear in the table on the page. Per marker the timestamp, message, number of logs and timespan are displayed. It is also possible to include the video file belonging to the marker. This is achieved by checking **Include Video**.

The name as well as the description of the entry can be entered in the input field provided for this purpose below the list. The priority of the entry can be selected from a dropdown as well as the type of the task. The project key, means the project for which the ticket is intended, can also be selected from a drop-down list (**Note:** The project keys, the task type and the priority levels are retrieved from JIRA and displayed there).

#### What happens when a ticket is created?
All log data and resource usage data associated with the markers are attached to the JIRA Backlog entry as a .txt or .json file. One log file is created and attached per marker. This means that if 3 markers are attached, the backlog entry will contain 3 log files. If **Include Video** is selected, a .mp4 file will also be attached. The text file is named after the device and the unique marker ID. The video file is named after the marker id and the device name. 

The ticket .txt or .json file can be found in the Documents directory. The video is saved in your configured video directory.

![backlogentry](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/ticketpage.PNG)

#### How is the .txt file structured?
All information like device, time span, marker ID and marker message are in the header of the file. Between the log data the maker is inserted at its set place. 
To find the marker despite a large number of log data, it is marked with<br>
 **###yourjiraprojectkey###**<br>
This makes it much easier to find the slot the marker was set.

Resource usage can be found with<br>
    **##yourjiraprojectkey##**<br>
    
#### How is the .json file structured?
Each object contains 13 values:

Not nullable:
<table>
  <tr>
    <td>Timestamp</td>
    <td>Host time of creation</td>
  </tr>
  <tr>
    <td>DeviceName</td>
    <td>Name of the device </td>
  </tr>
  <tr>
    <td>DeviceSerial</td>
    <td>Serial number of the device</td>
  </tr>
</table>

Nullable:
<table>
  <tr>
    <td>Message</td>
    <td>Message of the log</td>
  </tr>
  <tr>
    <td>DeviceTimestamp</td>
    <td>Device time of creation</td>
  </tr>
  <tr>
    <td>Pid</td>
    <td>Process ID</td>
  </tr>
  <tr>
    <td>Tid</td>
    <td>Thread ID</td>
  </tr>
  <tr>
    <td>LogLevel</td>
    <td>Level of Log</td>
  </tr>
  <tr>
    <td>App</td>
    <td>App that caused the log</td>
  </tr>
  <tr>
    <td>Cpu</td>
    <td>CPU usage of the android device</td>
  </tr>
  <tr>
    <td>Memory</td>
    <td>Memory usage of the android device</td>
  </tr>
  <tr>
    <td>Battery</td>
    <td>Battery status of the android device</td>
  </tr>
  <tr>
    <td>MarkerId</td>
    <td>Id of the marke</td>
  </tr>
</table>

### Config

To connect a device via network it is required to add the device to the known devices list. Therefore the IP-address of the device is needed. The IP-address can be entered in the input field and the device is added by pressing **Add Device**.

![adddevice](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/addevice.PNG)

In the Danger Zone, the ADB Server can be killed and restarted if any errors occur. Also the database can be cleared (all data will be lost. Forever!).

![highway to the danger zone](https://github.com/jsx15/AndroidDataRecorder/blob/main/screenshots/dangerzone.PNG)

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
