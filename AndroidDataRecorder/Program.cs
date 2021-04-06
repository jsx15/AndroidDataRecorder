using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndroidDataRecorder.Backend;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace AndroidDataRecorder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ///<summary>
            /// test connection for the database
            /// </summary>
            Database.Database database = new Database.Database();
            database.ConectionToDatabase();

            ///<summary>
            /// Methods for the Table Resource
            /// </summary>
            //database.InsertValuesInTableResources("device001", 22, 54, 66,DateTime.Now);
            //database.showAllEntries();

            Config.LoadConfig();

            //database.InsertValuesInMarker("device000", DateTime.Now);

          
            ///<summary>
            /// Methods for the table Marker
            /// </summary>
            //database.InsertValuesInTableMarker("device00222", DateTime.Now);
            //database.SearchMarkerTableByDeviceName("device00222");

            /*
            foreach (var marker in database.ListWithMarker("device00222"))
            {
                Console.WriteLine(marker.MarkerId.ToString() + " " + marker.DeviceName + " " + marker.Timestamp.ToString());
            } 
            */
            
            ///<summary>
            /// Methods for the table Logs
            /// </summary>
            ///database.InsertValuesInTableLogs("device123", DateTime.Now, DateTime.Now, 12342, 34521, "I", "App","Message" );
            /*foreach (var log in database.ListWithLogs())
            {
                Console.WriteLine(log._deviceName.ToString() + " " + log._deviceTimestamp.ToString());

            }*/
            
            // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        } 
    }
