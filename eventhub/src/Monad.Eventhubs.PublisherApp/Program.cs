using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Monad.Eventhubs.Core;
using System.Collections.Generic;
using System;

namespace Monad.Eventhubs.PublisherApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = Program.Intitialize();
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

            var storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={appSettings.StorageAccounName};AccountKey={appSettings.StorageAccountKey};EndpointSuffix=core.windows.net";

            // publisher code 
            var publisher = new Publisher();
            publisher.Init(appSettings.PublisherConnectionStrings);

            var random = new Random(Environment.TickCount);
            const int numEvents = 5;


            for (var i = 0; i < numEvents; i++)
            {
                var deviceTelemetry = DeviceTelemetry.GenerateRandom(random);
                publisher.PublishAsync(deviceTelemetry).GetAwaiter().GetResult();
                Console.WriteLine($"Published {i + 1} events...");
                Console.WriteLine($"{deviceTelemetry.IpAddress}  events...");

            }

            Console.WriteLine("Press any key to terminate the application");
            Console.Read();
        }

        private static IServiceProvider Intitialize()
        {
            var environmentName = Environment.GetEnvironmentVariable("EVENT_HUB_ENVIRONMENT");
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                 .AddEnvironmentVariables();

            var configuration = builder.Build();
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            return services.BuildServiceProvider();
        }
    }
}
