using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moand_IoT.Common;
using Newtonsoft.Json;

namespace Moand_IoT.MessageProcessor
{
    class Program
    {
        private static ILogger<Program> _logger;
        private static IServiceProvider serviceProvider;


        static async Task Main(string[] args)
        {
            serviceProvider = Bootstrapper.Intitialize();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            SetupLoggingProvider(loggerFactory);

            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;


            var processor = new EventProcessorHost( appSettings.IoTHubName,  appSettings.ConsumerGroupName, appSettings.IoTHubConnectionString,appSettings.StorageConnectionString, appSettings.StorageContainerName);

            await processor.RegisterEventProcessorAsync<CustomMessageProcessor>();

            Console.WriteLine("Event processor started, press enter to exit...");

            Console.ReadLine();

            await processor.UnregisterEventProcessorAsync();
        }

        private static void SetupLoggingProvider(ILoggerFactory factory)
        {
            //factory.AddConsole();
            //factory.AddDebug();
        }

    }
}
