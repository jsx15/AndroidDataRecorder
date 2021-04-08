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
            
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}