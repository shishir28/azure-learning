using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Monad.Eventhubs.Core;
using System;

namespace Monad.Eventhubs.DataPersistenceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = Program.Intitialize();
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
            var storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={appSettings.StorageAccounName};AccountKey={appSettings.StorageAccountKey};EndpointSuffix=core.windows.net";

            // consumer code                        
            var eventProcessorHost = new EventProcessorHost(appSettings.EventHubName,
                    "faster", appSettings.EventHubConnectionStrings, storageConnectionString, appSettings.StorageAccounName);
            var options = new EventProcessorOptions();
            options.SetExceptionHandler((x) => { Console.WriteLine(x.Exception); });
            // register the event host 
            // eventProcessorHost.RegisterEventProcessorAsync<DataPersistenceConsumer>(options).Wait();
            eventProcessorHost.RegisterEventProcessorFactoryAsync(new DataPersistenceConsumerFactory(appSettings.DatabaseConnectionStrings));

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
