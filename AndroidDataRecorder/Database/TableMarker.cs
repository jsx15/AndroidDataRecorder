using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Database
{
    public class TableMarker
    {
        private Database db = new Database();
        
        /// <summary>
        /// Insert the Values into table Marker
        /// </summary>
        /// <param name="deviceSerial"></param>
        /// <param name="deviceName"></param>
        /// <param name="markerMessage"></param>
        /// <param name="timestamp"></param>
        public void InsertValues(string deviceSerial, string deviceName, DateTime timestamp, string markerMessage)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
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

            try
            {
                // Execute Query
                command.ExecuteNonQuery();
            }
            catch
            {
                throw new SQLiteException("System can not insert the values in the database.");
            }
        }
        
        /// <summary>
        /// Creates a list of Marker
        /// </summary>
        /// <returns>markerList</returns>
        public List<Marker> GetList()
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Marker";

            // fill the list with the actual values of database
            List<Marker> markerList = new List<Marker>();

            try
            {
                // init new reader
                SQLiteDataReader reader = command.ExecuteReader();

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
            }
            catch
            {
                throw new SQLiteException("System can not access the values in the database.");
            }

            return markerList;
        }

        /// <summary>
        /// Method which generate a List of Marker by searching specific Marker and return it
        /// </summary>
        /// <param name="serial"></param>
        /// <returns> ListOfMarker </returns>
        public List<Marker> GetList(string serial)
        {
            if (serial.Equals(string.Empty))
            {
                return GetList();
            }

            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Marker
               WHERE Serial LIKE @serial";

            // use the Parameter DeviceName to search for it
            command.Parameters.AddWithValue("@Serial", serial);

            // fill the list with the actual values of database
            List<Marker> markerList = new List<Marker>();

            try
            {
                // init new reader
                SQLiteDataReader reader = command.ExecuteReader();

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
            }
            catch
            {
                throw new SQLiteException("System can not access the values in the database.");
            }

            return markerList;
        }
        
        /// <summary>
        /// Delete the Marker which is equal with the param markerId
        /// </summary>
        /// <param name="markerId"></param>
        public void DeleteRow(int markerId)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();
            //insert Query
            command.CommandText =
                @"DELETE FROM Marker
                    WHERE MarkerID = @markerID";

            // use the Parameter DeviceName to search for it
            command.Parameters.AddWithValue("@markerID", markerId);

            try
            {
                // Execute Query
                command.ExecuteNonQuery();
            }
            catch
            {
                throw new SQLiteException("System can not access the values in the database.");
            }

        }
        
        
    }
}