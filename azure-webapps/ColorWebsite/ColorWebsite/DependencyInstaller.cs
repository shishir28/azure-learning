using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using ColorWebsite.Data;
using ColorWebsite.Data.Services;

namespace ColorWebsite
{
    public static class DependencyInstaller
    {
        public static string DBConnectionString { get; set; }

        public static void InjectDependencies(IServiceCollection services, IConfigurationRoot configuration, ILoggerFactory factory)
        {
            var connString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseLoggerFactory(factory);
                options.UseSqlServer(connString, opt =>
                {
                });
            });
            services.AddTransient<IColorService, ColorService>();
        }

    }
}
