using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Database
{
    public class TableLogs
    {
        private Database db = new Database();
        
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
        public void InsertValues(string deviceSerial, string deviceName, DateTime systemTimestamp,
            DateTime deviceTimestamp, int pid, int tid, string loglevel, string app, string logMessage)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
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

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch
            {
                throw new SQLiteException("System can not insert the values in the database.");
            }
        }
        
        /// <summary>
        /// Returns a List with every Log in the table log
        /// </summary>
        /// <returns>LogList</returns>
        public List<LogEntry> GetList()
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Logs";

            // insert values into the list
            List<LogEntry> logsList = new List<LogEntry>();

            try
            {
                // init new reader
                SQLiteDataReader reader = command.ExecuteReader();

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
            }
            catch
            {
                throw new SQLiteException("System can not access the values in the database.");
            }

            return logsList;

        }
        
        /// <summary>
        /// Returns a List of Logs filtered by DeviceName
        /// </summary>
        /// <param name="serial"></param>
        /// <returns>LogList</returns>
        public List<LogEntry> GetList(String serial)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
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
                throw new SQLiteException("System can not access the values in the database.");
            }

            return logList;
        }
        
        /// <summary>
        /// Create a List of Logs filtered by logs
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="timeStamp1"></param>
        /// <param name="timeStamp2"></param>
        /// <param name="loglevel"></param>
        /// <returns>logList</returns>
        public List<LogEntry> GetList(string serial, DateTime timeStamp1, DateTime timeStamp2, string loglevel)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            switch (loglevel)
            {
                case "*":
                {
                    //insert Query
                    command.CommandText =
                        @"SELECT * FROM Logs
                    WHERE Serial LIKE @serial AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";
                    
                    // use the Parameter to search for it
                    command.Parameters.AddWithValue("@serial", serial);
                    command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                    command.Parameters.AddWithValue("@timeStamp2", timeStamp2);

                    break;
                }
                case "F":
                {
                    //insert Query
                    command.CommandText =
                        @"SELECT * FROM Logs
                    WHERE Serial LIKE @serial AND Loglevel LIKE 'F' AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";

                    //use the Parameter to search for it
                    command.Parameters.AddWithValue("@serial", serial);
                    command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                    command.Parameters.AddWithValue("@timeStamp2", timeStamp2);

                    break;
                }

                case "E":
                {
                    //insert Query
                    command.CommandText =
                        @"SELECT * FROM Logs
                    WHERE Serial LIKE @serial AND Loglevel LIKE 'E' OR Loglevel LIKE 'F' 
                    AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";

                    //use the Parameter to search for it
                    command.Parameters.AddWithValue("@serial", serial);
                    command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                    command.Parameters.AddWithValue("@timeStamp2", timeStamp2);

                    break;
                }

                case "W":
                {
                    //insert Query
                    command.CommandText =
                        @"SELECT * FROM Logs
                    WHERE  Serial LIKE @serial AND Loglevel LIKE 'F' OR Loglevel LIKE 'E' OR Loglevel LIKE 'W' 
                    AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";

                    //use the Parameter to search for it
                    command.Parameters.AddWithValue("@serial", serial);
                    command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                    command.Parameters.AddWithValue("@timeStamp2", timeStamp2);

                    break;
                }

                case "I":
                {
                    //insert Query
                    command.CommandText =
                        @"SELECT * FROM Logs
                    WHERE Serial LIKE @serial AND Loglevel LIKE 'F' OR Loglevel LIKE 'E' OR Loglevel LIKE'W' 
                    OR Loglevel LIKE 'I' AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";

                    //use the Parameter to search for it
                    command.Parameters.AddWithValue("@serial", serial);
                    command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                    command.Parameters.AddWithValue("@timeStamp2", timeStamp2);

                    break;
                }

                case "D":
                {
                    //insert Query
                    command.CommandText =
                        @"SELECT * FROM Logs
                    WHERE  Serial LIKE @serial AND Loglevel LIKE 'F' OR Loglevel LIKE 'E' OR Loglevel LIKE 'W' 
                       OR Loglevel LIKE 'I' OR Loglevel LIKE 'D' AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";

                    //use the Parameter to search for it
                    command.Parameters.AddWithValue("@serial", serial);
                    command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                    command.Parameters.AddWithValue("@timeStamp2", timeStamp2);

                    break;
                }

                case "V":
                {
                    //insert Query
                    command.CommandText =
                        @"SELECT * FROM Logs
                    WHERE Serial LIKE @serial AND SystemTimestamp BETWEEN @timeStamp1 AND @timeStamp2";

                    //use the Parameter to search for it
                    command.Parameters.AddWithValue("@serial", serial);
                    command.Parameters.AddWithValue("@timeStamp1", timeStamp1);
                    command.Parameters.AddWithValue("@timeStamp2", timeStamp2);

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
                throw new SQLiteException($"System can not access the values in the database.");
            }

            return logList;
        }
        
    }
}