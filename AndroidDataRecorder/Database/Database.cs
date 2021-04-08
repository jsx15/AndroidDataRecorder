using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AndroidDataRecorder.Misc;
using SharpAdbClient.Logs;

namespace AndroidDataRecorder.Database
{

    public class Database

    {
        private readonly string _datasource = "Data Source = " + System.IO.Path.GetFullPath(System.IO.Path.Combine(
            Environment.CurrentDirectory,
            ".." + System.IO.Path.DirectorySeparatorChar + "identifier.sqlite"));

        /// <summary>
        /// Path for the Database
        /// </summary>
        //private string datasource = "Data Source = C:/Users/sandra/Desktop/projekt/AndroidDataRecorder/identifier.sqlite";

        /// <summary>
        /// Create a object for the Connection to the Database, the method needs the path to the database
        /// </summary>
        /// <param name="datasource"></param>
        /// <returns> connection </returns>
        public SQLiteConnection ConectionToDatabase()
        {
            var connection = new SQLiteConnection(_datasource);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Insert the Values devicename, cpu, memory, timestamp to the table Resource.
        /// Just for testing
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="cpu"></param>
        /// <param name="memory"></param>
        /// <param name="timestamp"></param>
        public void InsertValuesInTableResources(string deviceName, int CPU, int Memory, int Battery,
            DateTime Timestamp)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO Resources(DeviceName, CPU, Memory, Battery, Timestamp)
                VALUES (@DeviceName, @CPU, @Memory,@Battery, @Timestamp)";

            // Define paramters to insert new values in the table
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);
            SQLiteParameter p2 = new SQLiteParameter("@CPU", DbType.Int32);
            SQLiteParameter p3 = new SQLiteParameter("@Memory", DbType.Int32);
            SQLiteParameter p4 = new SQLiteParameter("Battery", DbType.Int32);
            SQLiteParameter p5 = new SQLiteParameter("@Timestamp", DbType.DateTime);

            // Add the paramters to the table
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);
            command.Parameters.Add(p4);
            command.Parameters.Add(p5);

            // define the Values which will be insert to the table
            p1.Value = deviceName;
            p2.Value = CPU;
            p3.Value = Memory;
            p4.Value = Battery;
            p5.Value = Timestamp;

            //Execute Query
            command.ExecuteNonQuery();
        }
        
        ///<summary>
        /// Methods for the table Log
        /// </summary>

        /// <summary>
        /// Insert the Values devicname and Timestamp into the table Marker
        /// </summary>
        /// <param name="DeviceName"></param>
        /// <param name="Timestamp"></param>
        public void InsertValuesInTableMarker(string DeviceName, DateTime Timestamp, string markerMessage)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO Marker(DeviceName, Timestamp, MarkerMessage)
                VALUES (@DeviceName, @Timestamp, @MarkerMessage)";

            // Define paramters to insert new values in the table
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);
            SQLiteParameter p2 = new SQLiteParameter("@Timestamp", DbType.DateTime);
            SQLiteParameter p3 = new SQLiteParameter("@MarkerMessage", DbType.String);

            // Add the paramters to the table
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);

            // define the Values which will be insert to the table
            p1.Value = DeviceName;
            p2.Value = Timestamp;
            p3.Value = markerMessage;

            // Execute Query
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Method which generate a List of Marker by searching specific Marker and return it
        /// </summary>
        /// <param name="DeviceName"></param>
        /// <returns> ListOfMarker </returns>
        public List<Marker> ListWithMarker(string DeviceName)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Marker
               WHERE DeviceName LIKE @DeviceName";

            // use the Parameter DeviceName to search for it
            command.Parameters.AddWithValue("@DeviceName", DeviceName);

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actuall values of database
            List<Marker> MarkerList = new List<Marker>();

            while (reader.Read())
            {
                MarkerList.Add(new Marker()
                {
                    devicename = reader.GetString(1),
                    timeStamp = reader.GetDateTime(2),
                    message = reader.GetString(3)

                });
            }

            return MarkerList;
        }

        public void DeleteMarker(int markerid)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();
            //insert Query
            command.CommandText =
                @"DELETE FROM Marker
                    WHERE MarkerID = @markeriD";
            
            // use the Parameter DeviceName to search for it
            command.Parameters.AddWithValue("@markerid", markerid);

            // Execute Query
            command.ExecuteNonQuery();
            
        }

        /// <summary>
        /// Methods for the table Log
        /// </summary>

        /// <summary>
        /// Insert Values Into the Table Logs
        /// </summary>
        /// <param name="DeviceName"></param>
        /// <param name="SystemTimestamp"></param>
        /// <param name="DeviceTimestamp"></param>
        /// <param name="PID"></param>
        /// <param name="TID"></param>
        /// <param name="Loglevel"></param>
        /// <param name="APP"></param>
        /// <param name="LogMessage"></param>
        public void InsertValuesInTableLogs(string DeviceName, DateTime SystemTimestamp, DateTime DeviceTimestamp,
            int PID, int TID, string Loglevel, string APP, string LogMessage)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO Logs(DeviceName, SystemTimestamp, DeviceTimestamp, PID, TID, Loglevel, APP, LogMessage)
                VALUES (@DeviceName, @SystemTimestamp, @DeviceTimestamp,@PID, @TID, @Loglevel, @APP, @LogMessage)";

            // Define paramters to insert new values in the table
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);
            SQLiteParameter p2 = new SQLiteParameter("@SystemTimestamp", DbType.DateTime);
            SQLiteParameter p3 = new SQLiteParameter("@DeviceTimestamp", DbType.DateTime);
            SQLiteParameter p4 = new SQLiteParameter("PID", DbType.Int32);
            SQLiteParameter p5 = new SQLiteParameter("@TID", DbType.Int32);
            SQLiteParameter p6 = new SQLiteParameter("@Loglevel", DbType.String);
            SQLiteParameter p7 = new SQLiteParameter("@APP", DbType.String);
            SQLiteParameter p8 = new SQLiteParameter("@LogMessage", DbType.String);

            // Add the paramters to the table
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);
            command.Parameters.Add(p4);
            command.Parameters.Add(p5);
            command.Parameters.Add(p6);
            command.Parameters.Add(p7);
            command.Parameters.Add(p8);

            // define the Values which will be insert to the table
            p1.Value = DeviceName;
            p2.Value = SystemTimestamp;
            p3.Value = DeviceTimestamp;
            p4.Value = PID;
            p5.Value = TID;
            p6.Value = Loglevel;
            p7.Value = APP;
            p8.Value = LogMessage;

            //Execute Query
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns a List with every Log in the table log
        /// </summary>
        /// <returns>LogList</returns>
        public List<Backend.LogEntry> ListWithLogs()
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Logs";

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            List<Backend.LogEntry> LogsList = new List<Backend.LogEntry>();

            while (reader.Read())
            {
                LogsList.Add(new Backend.LogEntry(reader.GetString(1), reader.GetDateTime(2), reader.GetDateTime(3),
                    reader.GetInt32(4), reader.GetInt32(5), reader.GetString(6), reader.GetString(7),
                    reader.GetString(8)));
            }

            return LogsList;

        }
        
        /// <summary>
        /// Returns a List of Logs filtered by DeviceName
        /// </summary>
        /// <param name="device"></param>
        /// <returns>LogList</returns>
        public List<Backend.LogEntry> LogListFilterByDevice(String device)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();
            
            // Query for the Parameter device, with if else condition
            if (device.Equals("*") || device.Equals("") || device.StartsWith(""))
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
                      WHERE DeviceName LIKE @device";
                // use the Parameter DeviceName to search for it
                command.Parameters.AddWithValue("@device", device);
            }

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actuall values of database
            List<Backend.LogEntry> LogList = new List<Backend.LogEntry>();

            while (reader.Read())
            {
                LogList.Add(new Backend.LogEntry(reader.GetString(1), reader.GetDateTime(2), 
                    reader.GetDateTime(3), reader.GetInt32(4), reader.GetInt32(5), 
                    reader.GetString(6), reader.GetString(7), reader.GetString(8)));
            }

            return LogList;
        }

        public List<Backend.LogEntry> LogListFilterByTimestampAndLogLevel(DateTime timeStamp1, DateTime timeStamp2, string loglevel)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();
            
            
            
            //insert Query
            command.CommandText =
                @"SELECT * FROM Logs
                    WHERE Loglevel LIKE @loglevel AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";
            
            // use the Parameter DeviceName to search for it
            command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
            command.Parameters.AddWithValue("@timeStamp2", timeStamp2);
            command.Parameters.AddWithValue("@loglevel", loglevel);
            
            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actuall values of database
            List<Backend.LogEntry> LogList = new List<Backend.LogEntry>();

            while (reader.Read())
            {
                LogList.Add(new Backend.LogEntry(reader.GetString(1), reader.GetDateTime(2),
                    reader.GetDateTime(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetString(6), 
                    reader.GetString(7), reader.GetString(8)));
            }

            return LogList;
        }

        ///<summary>
        /// Methods for the table ResIntens
        /// </summary>
        public void InsertValuesIntoTableResIntens(int cpu, int memory, string app)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO ResIntens(CPU, Memory, App)
                VALUES (@CPU, @Memory, @App)";

            // Define paramters to insert new values in the table
            SQLiteParameter p1 = new SQLiteParameter("@CPU", DbType.Int32);
            SQLiteParameter p2 = new SQLiteParameter("@Memory", DbType.Int32);
            SQLiteParameter p3 = new SQLiteParameter("@app", DbType.String);
            
            // Add the paramters to the table
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);
            
            // define the Values which will be insert to the table
            p1.Value = cpu;
            p2.Value = memory;
            p3.Value = app;
            
            //Execute Query
            command.ExecuteNonQuery();
            
            
        }
        
        public List<ResIntensList> ResourcesIntensLists()
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();
            
            
                //insert Query
                command.CommandText =
                    @"SELECT * FROM ResIntens";
                

            // init new reader
            SQLiteDataReader reader = command.ExecuteReader();

            // fill the list with the actuall values of database
            List<ResIntensList> resourcesIntensListsList = new List<ResIntensList>();

            while (reader.Read())
            {
                resourcesIntensListsList.Add(new ResIntensList(reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3)));
            }

            return resourcesIntensListsList;
        }

    }
}
