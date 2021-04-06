using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndroidDataRecorder.Backend;
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
            ///<summary>
            /// test connection for the database
            /// </summary>
            //Database.Database database = new Database.Database();
            //database.ConectionToDatabase();

            Config.LoadConfig();
            // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        } 
    }
