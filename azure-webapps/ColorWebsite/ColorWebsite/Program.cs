using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ColorWebsite.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace ColorWebsite
{
    public class Program
    {

        public static void Main(string[] args)
        {
            BuildWebHost(args)
             .MigrateDbContext<ApplicationDBContext>((context, services) =>
             {
                 new ApplicationDbContextSeed()
                     .SeedAsync(context, services)
                     .Wait();
             })
               .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
                 .UseKestrel()
                 .UseIISIntegration()
                 .UseStartup<Startup>()
                 .ConfigureLogging((hostingContext, builder) =>
                 {
                     builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     builder.AddConsole();
                     builder.AddDebug();
                 })
                 .Build();

    }
}
