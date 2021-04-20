using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Database
{
    public class Database

    {
        /// <summary>
        /// Path to the Database
        /// </summary>
        private readonly string _datasource = "Data Source = " + System.IO.Path.GetFullPath(
            System.IO.Path.Combine(Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar +
                                   "identifier.sqlite"));

        /// <summary>
        /// Create a object for the Connection to the Database, the method needs the path to the database
        /// </summary>
        /// <returns> connection </returns>
        public SQLiteConnection ConnectionToDatabase()
        {
            var connection = new SQLiteConnection(_datasource);
            connection.Open();
            return connection;
        }

        ///<summary>
        /// Methods for the table Resources
        /// </summary>

        /// <summary>
        /// Insert the Values deviceName, cpu, memory, timestamp to the table Resource.
        /// Just for testing
        /// </summary>
        /// <param name="deviceSerial"></param>
        /// <param name="deviceName"></param>
        /// <param name="cpu"></param>
        /// <param name="memory"></param>
        /// <param name="battery"></param>
        /// <param name="timestamp"></param>
        public void InsertValuesInTableResources(string deviceSerial, string deviceName, int cpu, int memory,
            int battery,
            DateTime timestamp)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO Resources(Serial, DeviceName, CPU, Memory, Battery, Timestamp)
                VALUES (@Serial, @DeviceName, @CPU, @Memory,@Battery, @Timestamp)";

            // Define parameters to insert new values in the table
            SQLiteParameter p0 = new SQLiteParameter("@Serial", DbType.String);
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);
            SQLiteParameter p2 = new SQLiteParameter("@CPU", DbType.Int32);
            SQLiteParameter p3 = new SQLiteParameter("@Memory", DbType.Int32);
            SQLiteParameter p4 = new SQLiteParameter("@Battery", DbType.Int32);
            SQLiteParameter p5 = new SQLiteParameter("@Timestamp", DbType.DateTime);

            // Add the parameters to the table
            command.Parameters.Add(p0);
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);
            command.Parameters.Add(p4);
            command.Parameters.Add(p5);

            // define the Values which will be insert to the table
            p0.Value = deviceSerial;
            p1.Value = deviceName;
            p2.Value = cpu;
            p3.Value = memory;
            p4.Value = battery;
            p5.Value = timestamp;

            //Execute Query
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a list of Resources Table
        /// </summary>
        /// <param name="serial"></param>
        /// <returns> resList </returns>
        public List<ResourcesList> ResourcesLists(string serial)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Resources
                    WHERE Serial LIKE @serial";
            command.Parameters.AddWithValue("@Serial", serial);

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actual values of database
            List<ResourcesList> resList = new List<ResourcesList>();

            while (reader.Read())
            {
                resList.Add(new ResourcesList(
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetInt32(3),
                    reader.GetInt32(4),
                    reader.GetInt32(5),
                    reader.GetDateTime(6)));
            }

            return resList;
        }

        ///<summary>
        /// Methods for the table Marker
        /// </summary>

        /// <summary>
        /// Insert the Values deviceName and Timestamp into the table Marker
        /// </summary>
        /// <param name="deviceSerial"></param>
        /// <param name="deviceName"></param>
        /// <param name="markerMessage"></param>
        /// <param name="timestamp"></param>
        public void InsertValuesInTableMarker(string deviceSerial, string deviceName, DateTime timestamp,
            string markerMessage)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO Marker(Serial, DeviceName, Timestamp, MarkerMessage)
                VALUES (@Serial, @DeviceName, @Timestamp, @MarkerMessage)";

            // Define parameters to insert new values in the table
            SQLiteParameter p0 = new SQLiteParameter("@Serial", DbType.String);
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);
            SQLiteParameter p2 = new SQLiteParameter("@Timestamp", DbType.DateTime);
            SQLiteParameter p3 = new SQLiteParameter("@MarkerMessage", DbType.String);

            // Add the parameters to the table
            command.Parameters.Add(p0);
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);

            // define the Values which will be insert to the table
            p0.Value = deviceSerial;
            p1.Value = deviceName;
            p2.Value = timestamp;
            p3.Value = markerMessage;

            // Execute Query
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Method which generate a List of Marker by searching specific Marker and return it
        /// </summary>
        /// <param name="serial"></param>
        /// <returns> ListOfMarker </returns>
        public List<Marker> ListWithMarker(string serial)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Marker
               WHERE Serial LIKE @serial";

            // use the Parameter DeviceName to search for it
            command.Parameters.AddWithValue("@Serial", serial);

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actual values of database
            List<Marker> markerList = new List<Marker>();

            while (reader.Read())
            {
                markerList.Add(new Marker()
                {
                    MarkerId = reader.GetInt32(0),
                    deviceSerial = reader.GetString(1),
                    devicename = reader.GetString(2),
                    timeStamp = reader.GetDateTime(3),
                    message = reader.GetString(4)
                });
            }

            return markerList;
        }
        
        /// <summary>
        /// Creates a list of Marker
        /// </summary>
        /// <returns>markerList</returns>
        public List<Marker> ListWithMarker()
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Marker";

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actual values of database
            List<Marker> markerList = new List<Marker>();

            while (reader.Read())
            {
                markerList.Add(new Marker()
                {
                    MarkerId = reader.GetInt32(0),
                    deviceSerial = reader.GetString(1),
                    devicename = reader.GetString(2),
                    timeStamp = reader.GetDateTime(3),
                    message = reader.GetString(4)
                });
            }

            return markerList;
        }

        /// <summary>
        /// Delte the Marker which is equal with the param markeId
        /// </summary>
        /// <param name="markerId"></param>
        public void DeleteMarker(int markerId)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();
            //insert Query
            command.CommandText =
                @"DELETE FROM Marker
                    WHERE MarkerID = @markerID";

            // use the Parameter DeviceName to search for it
            command.Parameters.AddWithValue("@markerID", markerId);

            // Execute Query
            command.ExecuteNonQuery();

        }

        /// <summary>
        /// Methods for the table Log
        /// </summary>

        /// <summary>
        /// Insert Values Into the Table Logs
        /// </summary>
        /// <param name="deviceSerial"></param>
        /// <param name="deviceName"></param>
        /// <param name="systemTimestamp"></param>
        /// <param name="deviceTimestamp"></param>
        /// <param name="pid"></param>
        /// <param name="tid"></param>
        /// <param name="loglevel"></param>
        /// <param name="app"></param>
        /// <param name="logMessage"></param>
        public void InsertValuesInTableLogs(string deviceSerial, string deviceName, DateTime systemTimestamp,
            DateTime deviceTimestamp,
            int pid, int tid, string loglevel, string app, string logMessage)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO Logs(Serial, DeviceName, SystemTimestamp, DeviceTimestamp, PID, TID, Loglevel, APP, LogMessage)
                VALUES (@Serial, @DeviceName, @SystemTimestamp, @DeviceTimestamp,@PID, @TID, @Loglevel, @APP, @LogMessage)";

            // Define parameters to insert new values in the table
            SQLiteParameter p0 = new SQLiteParameter("@Serial", DbType.String);
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);
            SQLiteParameter p2 = new SQLiteParameter("@SystemTimestamp", DbType.DateTime);
            SQLiteParameter p3 = new SQLiteParameter("@DeviceTimestamp", DbType.DateTime);
            SQLiteParameter p4 = new SQLiteParameter("@PID", DbType.Int32);
            SQLiteParameter p5 = new SQLiteParameter("@TID", DbType.Int32);
            SQLiteParameter p6 = new SQLiteParameter("@Loglevel", DbType.String);
            SQLiteParameter p7 = new SQLiteParameter("@APP", DbType.String);
            SQLiteParameter p8 = new SQLiteParameter("@LogMessage", DbType.String);

            // Add the parameters to the table
            command.Parameters.Add(p0);
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);
            command.Parameters.Add(p4);
            command.Parameters.Add(p5);
            command.Parameters.Add(p6);
            command.Parameters.Add(p7);
            command.Parameters.Add(p8);

            // define the Values which will be insert to the table
            p0.Value = deviceSerial;
            p1.Value = deviceName;
            p2.Value = systemTimestamp;
            p3.Value = deviceTimestamp;
            p4.Value = pid;
            p5.Value = tid;
            p6.Value = loglevel;
            p7.Value = app;
            p8.Value = logMessage;

            //Execute Query
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns a List with every Log in the table log
        /// </summary>
        /// <returns>LogList</returns>
        public List<LogEntry> ListWithLogs()
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Logs";

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            List<LogEntry> logsList = new List<LogEntry>();

            while (reader.Read())
            {
                logsList.Add(new LogEntry(
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDateTime(3),
                    reader.GetDateTime(4),
                    reader.GetInt32(5),
                    reader.GetInt32(6),
                    reader.GetString(7),
                    reader.GetString(8),
                    reader.GetString(9)));
            }

            return logsList;

        }

        /// <summary>
        /// Returns a List of Logs filtered by DeviceName
        /// </summary>
        /// <param name="serial"></param>
        /// <returns>LogList</returns>
        public List<LogEntry> LogListFilterByDevice(String serial)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            // Query for the Parameter device, with if else condition
            if (serial.Equals("*") || serial.Equals("") || serial.StartsWith(""))
            {
                //insert Query
                command.CommandText =
                    @"SELECT * FROM Logs";
            }

            else
            {
                //insert Query
                command.CommandText =
                    @"SELECT * FROM Logs
                      WHERE Serial LIKE @serial";
                // use the Parameter DeviceName to search for it
                command.Parameters.AddWithValue("@Serial", serial);
            }

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actual values of database
            List<LogEntry> logList = new List<LogEntry>();

            while (reader.Read())
            {
                logList.Add(new LogEntry(reader.GetString(1), reader.GetString(2), reader.GetDateTime(3),
                    reader.GetDateTime(4), reader.GetInt32(5), reader.GetInt32(6),
                    reader.GetString(7), reader.GetString(8), reader.GetString(9)));
            }

            return logList;
        }

        /// <summary>
        /// Creates a List of Log List, which is filtered by the params timeStamp1, timeStamp2 and Loglevel
        /// </summary>
        /// <param name="timeStamp1"></param>
        /// <param name="timeStamp2"></param>
        /// <param name="loglevel"></param>
        /// <returns>LogList</returns>
        public List<LogEntry> LogListFilterByTimestampAndLogLevel(DateTime timeStamp1, DateTime timeStamp2,
            string loglevel)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            if (loglevel.Equals("*") || loglevel.Equals("") || loglevel.StartsWith(" "))
            {
                //insert Query
                command.CommandText =
                    @"SELECT * FROM Logs
                    WHERE SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";

                // use the Parameter DeviceName to search for it
                command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                command.Parameters.AddWithValue("@timeStamp2", timeStamp2);
                command.Parameters.AddWithValue("@loglevel", loglevel);
            }
            else
            {
                //insert Query
                command.CommandText =
                    @"SELECT * FROM Logs
                    WHERE Loglevel LIKE @loglevel AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";
                // use the Parameter DeviceName to search for it
                command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                command.Parameters.AddWithValue("@timeStamp2", timeStamp2);
                command.Parameters.AddWithValue("@loglevel", loglevel);
            }


            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actual values of database
            List<LogEntry> logList = new List<LogEntry>();

            while (reader.Read())
            {
                logList.Add(new LogEntry(reader.GetString(1), reader.GetString(2), reader.GetDateTime(3),
                    reader.GetDateTime(4), reader.GetInt32(5), reader.GetInt32(6),
                    reader.GetString(7), reader.GetString(8), reader.GetString(9)));
            }

            return logList;
        }
        
        /// <summary>
        /// Create a List of Logs filtered by logs
        /// </summary>
        /// <param name="loglevel"></param>
        /// <returns>logList</returns>
        public List<LogEntry> LogListFilteredByLog(string loglevel) 
        {
          // create connection to the database
          var connection = ConnectionToDatabase();
          var command = connection.CreateCommand();

          switch (loglevel)
          {
              case "V":
              {
                  //insert Query
                  command.CommandText =
                      @"SELECT * FROM Logs
                    WHERE Loglevel LIKE 'V'";
                  break;
              }

              case "D":
              {
                  //insert Query
                  command.CommandText =
                      @"SELECT * FROM Logs
                    WHERE Loglevel LIKE 'V' OR Loglevel LIKE 'D'";
                  break;
              }

              case "I":
              {
                  //insert Query
                  command.CommandText =
                      @"SELECT * FROM Logs
                    WHERE  Loglevel LIKE 'V' OR Loglevel LIKE 'D' OR Loglevel LIKE 'I'";
                  break;
              }

              case "W":
              {
                  //insert Query
                  command.CommandText =
                      @"SELECT * FROM Logs
                    WHERE Loglevel LIKE 'V' OR Loglevel LIKE 'D' OR Loglevel LIKE'I' OR Loglevel LIKE 'W'";
                  break;
              }

              case "E":
              {
                  //insert Query
                  command.CommandText =
                      @"SELECT * FROM Logs
                    WHERE  Loglevel LIKE 'V' OR Loglevel LIKE 'D' OR Loglevel LIKE 'I' 
                       OR Loglevel LIKE 'W' OR Loglevel LIKE 'E' ";
                  break;
              }

              case "F":
              {
                  //insert Query
                  command.CommandText =
                      @"SELECT * FROM Logs
                    WHERE  Loglevel LIKE'F'";
                  break;
              }
          }
          
          // fill the list with the actual values of database
          List<LogEntry> logList = new List<LogEntry>();
          
          try
          {
              // init new reader
              SQLiteDataReader reader = command.ExecuteReader();

              while (reader.Read())
              {
                  logList.Add(new LogEntry(reader.GetString(1), reader.GetString(2), reader.GetDateTime(3),
                      reader.GetDateTime(4), reader.GetInt32(5), reader.GetInt32(6),
                      reader.GetString(7), reader.GetString(8), reader.GetString(9)));
              }
              
          }
          catch
          {
              // ReSharper disable once ObjectCreationAsStatement
              new SQLiteException($"System can not access the values in the database.");
          }
          
          return logList;
      }

      ///<summary>
        /// Methods for the table ResIntens
        /// </summary>

        ///<summary>
        /// Insert Values Into the table ResIntens
        /// </summary>
        public void InsertValuesIntoTableResIntens(string serial, string deviceName, double cpu, double memory,
            string process, DateTime timestamp)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO ResIntens(Serial, DeviceName, CPU, Memory, Process, Timestamp)
                VALUES (@Serial, @DeviceName, @CPU, @Memory, @Process, @Timestamp)";

            // Define parameters to insert new values in the table
            SQLiteParameter p0 = new SQLiteParameter("@Serial", DbType.String);
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);
            SQLiteParameter p2 = new SQLiteParameter("@CPU", DbType.Double);
            SQLiteParameter p3 = new SQLiteParameter("@Memory", DbType.Double);
            SQLiteParameter p4 = new SQLiteParameter("@Process", DbType.String);
            SQLiteParameter p5 = new SQLiteParameter("@Timestamp", DbType.DateTime);

            // Add the parameters to the table
            command.Parameters.Add(p0);
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);
            command.Parameters.Add(p4);
            command.Parameters.Add(p5);

            // define the Values which will be insert to the table
            p0.Value = serial;
            p1.Value = deviceName;
            p2.Value = cpu;
            p3.Value = memory;
            p4.Value = process;
            p5.Value = timestamp;

            //Execute Query
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns a list of the table ResIntens
        /// </summary>
        /// <returns>resourcesIntensLists</returns>
        public List<ResIntensList> ResourcesIntensLists(string serial)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM ResIntens
                    WHERE Serial LIKE @serial";
            command.Parameters.AddWithValue("@Serial", serial);

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actual values of database
            List<ResIntensList> resourcesIntensLists = new List<ResIntensList>();

            while (reader.Read())
            {
                resourcesIntensLists.Add(new ResIntensList(
                    reader.GetName(1),
                    reader.GetString(2),
                    reader.GetDouble(3),
                    reader.GetDouble(4),
                    reader.GetString(5),
                    reader.GetDateTime(6)));
            }

            return resourcesIntensLists;
        }
        
        ///<summary>
        /// Methods for the table Device
        /// </summary>
        
        /// <summary>
        /// Method to Insert Values Into the table Device
        /// </summary>
        /// <param name="serialDevice"></param>
        /// <param name="deviceName"></param>
        public void InsertValuesIntoDeviceTable(string serialDevice, string deviceName)
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO Devices(Serial, DeviceName)
                VALUES (@Serial, @DeviceName)";

            // Define parameters to insert new values in the table
            SQLiteParameter p0 = new SQLiteParameter("@Serial", DbType.String);
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);

            // Add the parameters to the table
            command.Parameters.Add(p0);
            command.Parameters.Add(p1);

            // define the Values which will be insert to the table
            p0.Value = serialDevice;
            p1.Value = deviceName;

            //Execute Query
            command.ExecuteNonQuery();

        }

        /// <summary>
        /// Method to return a list of the table device
        /// </summary>
        /// <returns></returns>
        public List<DeviceList> DeviceList()
        {
            // create connection to the database
            var connection = ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Devices";

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actual values of database
            List<DeviceList> deviceList = new List<DeviceList>();

            while (reader.Read())
            {
                deviceList.Add(new DeviceList(
                    reader.GetString(0),
                    reader.GetString(1)));

            }

            return deviceList;

        }
    }
}