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
            
            db.InsertValuesIntoTableResIntens(22,22, "hi", DateTime.Now);

            foreach (var v in db.ResourcesIntensLists())
            {
                Console.WriteLine(v.app + " " + v.timestamp);
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}