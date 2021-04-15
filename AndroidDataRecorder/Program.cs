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
            //Load data from the Config file and start the adb server
            Config.LoadConfig();
            
            //Create the razor pages
            CreateHostBuilder(args).Build().Run();
        }
        

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}