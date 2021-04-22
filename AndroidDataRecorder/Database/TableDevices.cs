using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using AndroidDataRecorder.Misc;

namespace AndroidDataRecorder.Database
{
    public class TableDevices
    {
        private Database db = new Database();
        
        /// <summary>
        /// Method to Insert Values Into the table Device
        /// </summary>
        /// <param name="serialDevice"></param>
        /// <param name="deviceName"></param>
        public void DeviceTable(string serialDevice, string deviceName)
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"INSERT INTO Devices(Serial, DeviceName)
                VALUES (@Serial, @DeviceName)";

            // Define parameters to insert new values in the table
            SQLiteParameter p0 = new SQLiteParameter("@Serial", DbType.String);
            SQLiteParameter p1 = new SQLiteParameter("@DeviceName", DbType.String);

            // Add the parameters to the table
            command.Parameters.Add(p0);
            command.Parameters.Add(p1);

            // define the Values which will be insert to the table
            p0.Value = serialDevice;
            p1.Value = deviceName;

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
        /// Method to return a list of the table device
        /// </summary>
        /// <returns></returns>
        public List<DeviceList> DeviceList()
        {
            // create connection to the database
            var connection = db.ConnectionToDatabase();
            var command = connection.CreateCommand();

            //insert Query
            command.CommandText =
                @"SELECT * FROM Devices";

            // fill the list with the actual values of database
            List<DeviceList> deviceList = new List<DeviceList>();

            try
            {
                // init new reader
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    deviceList.Add(new DeviceList(
                        reader.GetString(0),
                        reader.GetString(1)));
                }
            }
            catch
            {
                throw new SQLiteException("System can not access the values in the database.");
            }

            return deviceList;
        }
        
    }
}