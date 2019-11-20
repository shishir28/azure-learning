using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moand_IoT.Common;
using Newtonsoft.Json;

namespace Moand_IoT.Simulator
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

            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();

            Console.WriteLine("Initializing Band Agent...");

            var device = DeviceClient.CreateFromConnectionString(appSettings.Value.IoTHubConnectionString);

            await device.OpenAsync();

            Console.WriteLine("Device is connected!");
            await UpdateTwin(device);


            await PerformBandAction(device);

            Console.WriteLine("Disconnecting...");
        }

        private static async Task PerformBandAction(DeviceClient device)
        {

            Console.WriteLine("Press a key to perform an action:");
            Console.WriteLine("q: quits");
            Console.WriteLine("h: send happy feedback");
            Console.WriteLine("u: send unhappy feedback");
            Console.WriteLine("e: request emergency help");

            var random = new Random();
            var quitRequested = false;
            while (!quitRequested)
            {
                Console.Write("Action? ");
                var input = Console.ReadKey().KeyChar;
                Console.WriteLine();

                var status = StatusType.NotSpecified;
                var latitude = random.Next(0, 100);
                var longitude = random.Next(0, 100);

                switch (Char.ToLower(input))
                {
                    case 'q':
                        quitRequested = true;
                        break;
                    case 'h':
                        status = StatusType.Happy;
                        break;
                    case 'u':
                        status = StatusType.Unhappy;
                        break;
                    case 'e':
                        status = StatusType.Emergency;
                        break;
                }

                var telemetry = new Telemetry
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Status = status
                };

                var payload = JsonConvert.SerializeObject(telemetry);

                var message = new Message(Encoding.ASCII.GetBytes(payload));

                await device.SendEventAsync(message);

                Console.WriteLine("Message sent!");
            }


        }

        private static async Task UpdateTwin(DeviceClient device)
        {
            var twinProperties = new TwinCollection();
            twinProperties["connection.type"] = "wi-fi";
            twinProperties["connectionStrength"] = "full";

            await device.UpdateReportedPropertiesAsync(twinProperties);
        }

        private static void SetupLoggingProvider(ILoggerFactory factory)
        {
            //factory.AddConsole();
            //factory.AddDebug();
        }

    }
}