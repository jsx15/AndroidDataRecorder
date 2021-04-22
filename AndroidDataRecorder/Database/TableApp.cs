using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Database
{
    public class TableApp
    {
        private Database db = new Database();
        
        /// <summary>
        /// Insert Value into table App
        /// </summary>
        /// <param name="app"></param>
        /// <param name="serialFK"></param>
        /// <exception cref="SQLiteException"></exception>
        public void InsertValues(string app, string serialFK)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO App(AppName, Serial)
                VALUES (@app, @serialFK)";
            
            // Define parameters to insert new values in the table
            SQLiteParameter p0 = new SQLiteParameter("@app", DbType.String);
            SQLiteParameter p1 = new SQLiteParameter("@serialFK", DbType.String);

            // Add the parameters to the table
            command.Parameters.Add(p0);
            command.Parameters.Add(p1);

            // define the Values which will be insert to the table
            p0.Value = app;
            p1.Value = serialFK;

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch
            {
                throw new SQLiteException("Can not insert the Values into the Database");
            }

        }
        
        /// <summary>
        /// Create a AppList filtered By serial Number
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        /// <exception cref="SQLiteException"></exception>
        public List<AppList> GetList(string serial)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM App
                    WHERE Serial LIKE @serial
                    GROUP BY AppName, Serial";
            
            command.Parameters.AddWithValue("@Serial", serial);


            // fill the list with the actual values of database
            List<AppList> appList = new List<AppList>();

            try
            {
                // init new reader
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    appList.Add(new AppList(
                        reader.GetString(1),
                        reader.GetString(2)));
                }
            }
            catch
            {
                throw new SQLiteException("System can not access the values in the database.");
            }

            return appList;
        }
        
        /// <summary>
        /// Delete Rows in Table App which are Equal like the specified serial Number
        /// </summary>
        /// <param name="serial"></param>
        /// <exception cref="SQLiteException"></exception>
        public void DeleteRow(string serial)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();
            //insert Query
            command.CommandText =
                @"DELETE FROM App
                    WHERE Serial LIKE @serial";

            // use the Parameter DeviceName to search for it
            command.Parameters.AddWithValue("@Serial", serial);

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