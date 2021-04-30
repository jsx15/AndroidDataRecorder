using System;
using System.Data.SQLite;

namespace AndroidDataRecorder.Database
{
    public class DeleteAll
    {
        private Database db = new Database();
        
        /// <summary>
        /// Delete everything from the Database
        /// </summary>
        /// <exception cref="SQLiteException"></exception>
        public void Table()
        {
            
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"DELETE FROM App";

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new SQLiteException("Can not delete the the Inserts in App");
            }
            
            //insert Query
            command.CommandText =
                @"DELETE FROM Devices";

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new SQLiteException("Can not delete the the Inserts in Devices");
            }
            
            //insert Query
            command.CommandText =
                @"DELETE FROM Logs";

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new SQLiteException("Can not delete the the Inserts in Logs");
            }
            
            //insert Query
            command.CommandText =
                @"DELETE FROM Marker";

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new SQLiteException("Can not delete the the Inserts in Marker");
            }
            
            //insert Query
            command.CommandText =
                @"DELETE FROM ResIntens";

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new SQLiteException("Can not delete the the Inserts in ResIntens");
            } 
            
            //insert Query
            command.CommandText =
                @"DELETE FROM Resources";

            try
            {
                //Execute Query
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new SQLiteException("Can not delete the the Inserts in Resources");
            }
        }

    }
}