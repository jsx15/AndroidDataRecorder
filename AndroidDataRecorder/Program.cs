using System;
using AndroidDataRecorder.Backend;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AndroidDataRecorder
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //Load data from the Config file and start the adb server
            Config.LoadConfig();
            
            //Create the razor pages
            //CreateHostBuilder(args).Build().Run();

            Database.Database db = new Database.Database();

            foreach (var app in db.AppList())
            {
                Console.WriteLine(app.appName + " " + app.serialFK);
            }



        }
        

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}