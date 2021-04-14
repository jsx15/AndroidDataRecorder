using System;
using System.Collections.Generic;
using AndroidDataRecorder.Backend;
using AndroidDataRecorder.Misc;
using Blazored.Toast;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndroidDataRecorder
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredToast();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
/*
            DateTime before = DateTime.Now;
            Demo demo = new Demo();
            demo.allSelected = new List<Demo.combinedInfo>();

            Demo.combinedInfo info = new Demo.combinedInfo();
            info._marker = new Marker(5, "NameDemo4", DateTime.Now, "messageText");
            info._logs = new List<LogEntry>();
            info._logs.Add(new LogEntry("deviceName", before,
                DateTime.Now, 23, 12, "E", "App", "message"));
            info._logs.Add(new LogEntry("deviceName", before,
                DateTime.Now, 3, 2, "E", "App", "message"));
            info._logs.Add(new LogEntry("deviceName", DateTime.Now.Date, 
                before, 263, 1288, "E", "App", "message"));
            info.timeSpanMinus = 0.123;
            info.timeSpanPlus = 1.34566;
            
            Demo.combinedInfo combinedInfo = new Demo.combinedInfo();
            combinedInfo._marker = new Marker(2, "NameDemo", DateTime.Now, "messageText");
            combinedInfo._logs = new List<LogEntry>();
            combinedInfo._logs.Add(new LogEntry("deviceName", before,
                DateTime.Now, 23, 12, "E", "App", "message"));
            combinedInfo._logs.Add(new LogEntry("deviceName", before,
                DateTime.Now, 3, 2, "E", "App", "message"));
            combinedInfo._logs.Add(new LogEntry("deviceName", DateTime.Now, 
                before, 263, 1288, "E", "App", "message"));
            combinedInfo.timeSpanMinus = 0.123;
            combinedInfo.timeSpanPlus = 1.34566;
            
            demo.allSelected.Add(info);
            demo.allSelected.Add(combinedInfo);

            TicketCreator ticket = new TicketCreator();
            ticket.CreateTicketJson(demo.allSelected,"ADR",TicketCreator.TicketType.Bug,
                TicketCreator.TicketPriority.Low,"This is A Test Ticket created by Software","Erwin Kenner",
                "This Ticket was created by our Software.\n each Marker has its own Text file with logs associated to this marker");
            */
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}