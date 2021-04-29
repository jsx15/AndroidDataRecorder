using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Database
{
    public class TableResIntens
    {
        private Database db = new Database();
        
         ///<summary>
        /// Insert Values Into the table ResIntens
        /// </summary>
        public void InsertValues(string serial, string deviceName, double cpu, double memory,
            string process, DateTime timestamp)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
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
         /// Returns a list of the table ResIntens
         /// </summary>
         /// <param name="serial"></param>
         /// <returns>resourcesIntensLists</returns>
         public List<ResIntensList> GetList(string serial)
         {
             // create connection to the database
             var connection = db.ConnectionToDatabase();
             var command = connection.CreateCommand();

             //insert Query
             command.CommandText =
                 @"SELECT * FROM ResIntens
                    WHERE Serial LIKE @serial
                    ORDER BY ResId DESC LIMIT 5";
            
             // use the Parameter to search for it
             command.Parameters.AddWithValue("@Serial", serial);

             // fill the list with the actual values of database
             List<ResIntensList> resourcesIntensLists = new List<ResIntensList>();

             try
             {
                 // init new reader
                 SQLiteDataReader reader = command.ExecuteReader();

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
             }
             catch
             {
                 throw new SQLiteException("System can not access the values in the database.");
             }

             return resourcesIntensLists;
         }
         
        
    }
}