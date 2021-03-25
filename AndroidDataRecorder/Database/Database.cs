using System.Data.SQLite;
namespace AndroidDataRecorder.Database
{
    
    public class Database

    {
        private string dataSource = "C:/Users/sandra/Desktop/projekt/AndroidDataRecorder/identifier.sqlite";
        /// <summary>
        /// Create a variable for the Connection to the Database, the method needs the path to the database
        /// </summary>
        /// <param name="datasource"></param>
        /// <returns> connection </returns>
        public SQLiteConnection ConectionToDatabase(string datasource)
        {
            var connection = new SQLiteConnection("Data Source = " + datasource);
            return connection;
        }
        
        
        /// <summary>
        ///  Creates a command for the database to execute an SQL query
        /// </summary>
        /// <returns> command </returns>
        public SQLiteCommand SQLCommand()
        {
            var connection = ConectionToDatabase(dataSource);
            var command = connection.CreateCommand();
            return command;
        }
    }
}