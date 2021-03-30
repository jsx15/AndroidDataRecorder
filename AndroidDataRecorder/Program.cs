using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            //Database.Database database = new Database.Database();
            //database.ConectionToDatabase();
            
            ///<summary>
            /// Methods for the Table Resource
            /// </summary>
            //database.InsertValuesInTableResources("device001", 22, 54, 66,DateTime.Now);
            //database.showAllEntries();
            
            ///<summary>
            /// Methods for the table Marker
            /// </summary>
            //database.InsertValuesInMarker("device00222", DateTime.Now);
            //database.SearchMarkerTableByDeviceName("device00222");

            /*
            foreach (var marker in database.ListWithMarker("device00222"))
            {
                Console.WriteLine(marker.MarkerId.ToString() + " " + marker.DeviceName + " " + marker.Timestamp.ToString());
            } 
            */
            
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}