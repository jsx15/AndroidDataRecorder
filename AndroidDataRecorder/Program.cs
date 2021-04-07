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


            Config.LoadConfig();
            CreateHostBuilder(args).Build().Run();

            Database.Database db = new Database.Database();
            db.ConectionToDatabase();
            foreach (var log in db.LogListFilterByTimestampAndLogLevel
                (Convert.ToDateTime("2021-04-06 15:22:11.6396469"), Convert.ToDateTime("2021-04-06 15:54:22.05"), "I"))
            {
                Console.WriteLine(log.devicename + " " + log.timeStamp + log.LogLevel);
            }
        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}