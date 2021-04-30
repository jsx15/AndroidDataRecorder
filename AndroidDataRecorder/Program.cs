using AndroidDataRecorder.Backend;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using AndroidDataRecorder.Database;
using RestSharp;

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

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://*:5001", "http://*:5000");
                });
    }
}