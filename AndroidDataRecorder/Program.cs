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
             
            // test Connection to database
            
            //Database.Database database = new Database.Database();
            //database.ConectionToDatabase();
            //database.InsertValuesInTableResources("device000", 22, 54, 66,DateTime.Now);
            //database.showAllEntries();
           
            CreateHostBuilder(args).Build().Run();
           
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}