using System;
using System.Data;
using System.Data.SQLite;

namespace AndroidDataRecorder.Database
{

    public class Database

    {
        private string datasource = "Data Source = C:/Users/sandra/Desktop/projekt/AndroidDataRecorder/identifier.sqlite";

        /// <summary>
        /// Create a variable for the Connection to the Database, the method needs the path to the database
        /// </summary>
        /// <param name="datasource"></param>
        /// <returns> connection </returns>
        public SQLiteConnection ConectionToDatabase()
        {
            var connection = new SQLiteConnection(datasource);
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
        public void InsertValuesInTableResources(string deviceName, int CPU, int Memory, int Battery, DateTime Timestamp)
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
            p1.Value = string.Concat(deviceName);
                p2.Value = CPU;
                p3.Value = Memory;
                p4.Value = Battery;
                p5.Value = Timestamp;
                command.ExecuteNonQuery();
            
            
        }

        public void InsertValuesInMarker(string DeviceName, DateTime Timestamp)
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();
            
            //insert Query
            command.CommandText =
                @"INSERT INTO Marker(DeviceName, Timestamp)
                VALUES (@DeviceName, @Timestamp)";
            
            // Define paramters to insert new values in the table
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);
            SQLiteParameter p2 = new SQLiteParameter("@Timestamp", DbType.DateTime);
            
            // Add the paramters to the table
            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            
            // define the Values which will be insert to the table
            p1.Value = DeviceName;
            p2.Value = Timestamp;
            command.ExecuteNonQuery();

        }

        /// <summary>
        /// Shows all Entries of the table Resources
        /// </summary>
        public  void showAllEntries()
        {
            // create connection to the database
            var connection = ConectionToDatabase();
            var command = connection.CreateCommand();
            
            // Query to get the all values of the table
            command.CommandText = 
                @"SELECT * FROM Resources";
            
            // define a reader which get the entries 
            SQLiteDataReader reader = command.ExecuteReader();
            
            // get the head of all columns and print them out
            Console.WriteLine($"{reader.GetName(0), -3} " +
                              $"{reader.GetName(1), -9} " +
                              $"{reader.GetName(2), 8}" +
                              $" {reader.GetName(3), 10}" +
                              $"{reader.GetName(4)}");
            
            // print all entries
            while (reader.Read())
            {
                Console.WriteLine($@"{reader.GetString(0), -3}" + 
                                  $"{reader.GetInt32(1), -9}" + 
                                  $"{reader.GetInt32(2), 8}" + 
                                  $"{reader.GetInt32(3), 10}" +
                                  $"{reader.GetDateTime(4)}");
            }
        }
    }
}