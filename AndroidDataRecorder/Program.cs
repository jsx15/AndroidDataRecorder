using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AndroidDataRecorder.Backend;
using AndroidDataRecorder.Misc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace AndroidDataRecorder
{
    public class Program
    {
        public static void Main(string[] args)
        {


            Config.LoadConfig();
            CreateHostBuilder(args).Build().Run();

            Database.Database db = new Database.Database();
            db.ConnectionToDatabase();
            
            db.InsertValuesInTableResources("12345", "name", 1, 2, 3, DateTime.Now);
            
            
            var log = db.ResourcesIntensLists("name2").ElementAt(db.ResourcesIntensLists("name2").Count - 1);
            Console.WriteLine(log.cpu + " " + log.timestamp);

            var resourcesList = db.ResourcesLists("device001").ElementAt(db.ResourcesLists("device001").Count - 1);
            Console.WriteLine(resourcesList.deviceName + " " + resourcesList.timestamp);
            


        }
        

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}