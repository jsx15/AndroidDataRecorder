using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Database
{
    public class TableResources
    {
         private Database db = new Database();
         
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
        public void InsertValues(string deviceSerial, string deviceName, int cpu, int memory,
            int battery, DateTime timestamp)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
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

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch
            {
                throw new SQLiteException($"System can not insert the values in the database.");
            }
        }
        
        /// <summary>
        /// Creates a list of Resources Table filtered by serial Number
        /// </summary>
        /// <param name="serial"></param>
        /// <returns> resList </returns>
        public List<ResourcesList> GetList(string serial)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Resources
                    WHERE Serial LIKE @serial";
            command.Parameters.AddWithValue("@Serial", serial);

            // fill the list with the actual values of database
            List<ResourcesList> resList = new List<ResourcesList>();


            try
            {
                // init new reader
                SQLiteDataReader reader = command.ExecuteReader();

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
            }
            catch
            {
                throw new SQLiteException("System can not Insert the values in the database.");
            }

            return resList;
        }
        
        /// <summary>
        /// Creates a List of Resources table filtered by Serial Number and two timestamps
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        /// <exception cref="SQLiteException"></exception>
        public List<ResourcesList> GetList(string serial, DateTime t1, DateTime t2)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Resources
                    WHERE Serial LIKE @serial AND Timestamp BETWEEN  @t1 AND @t2";
            
            // define Paramaters
            command.Parameters.AddWithValue("@serial", serial);
            command.Parameters.AddWithValue("@t1", t1);
            command.Parameters.AddWithValue("@t2", t2);
            
            // fill the list with the actual values of database
            List<ResourcesList> resList = new List<ResourcesList>();


            try
            {
                // init new reader
                SQLiteDataReader reader = command.ExecuteReader();

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
            }
            catch
            {
                throw new SQLiteException("System can not insert the values in the database.");
            }

            return resList;

        }
        
        
    }
}