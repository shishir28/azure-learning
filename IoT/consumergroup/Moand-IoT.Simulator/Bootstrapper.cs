using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moand_IoT.Common;

namespace Moand_IoT.Simulator
{
    public static class Bootstrapper
    {
        public static IServiceProvider Intitialize()
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));


            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            return services.BuildServiceProvider();
        }
    }
}
