using System;
using System.Data.SQLite;

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
            SQLiteConnection connection;
            try
            {
                connection = new SQLiteConnection(_datasource);
                connection.Open();
            }
            catch
            {
                throw new SQLiteException("Connection refused");
            }

            return connection;
        }
    }
}