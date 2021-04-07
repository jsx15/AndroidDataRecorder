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
            //CreateHostBuilder(args).Build().Run();

            Database.Database db = new Database.Database();
            db.ConectionToDatabase();

            foreach (var logs in db.LogListFilterByTimestamp("2021-04-06 15:54:24.873"))
            {
                Console.WriteLine(logs._deviceName + " " + logs._systemTimestamp);
            }
            
            db.DeleteRowInTableMarker(3816);
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}