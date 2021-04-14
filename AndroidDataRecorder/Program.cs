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

            foreach (var dev in db.DeviceList())
            {
                Console.WriteLine(dev.serial);
            }
            
        }
        

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}