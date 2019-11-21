using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Moand_IoT.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moand_IoT.MessageProcessor
{
    public class AnotherSimulator
    {
        private DeviceClient _device;
        public AnotherSimulator(string connectionString)
        {
            _device  = DeviceClient.CreateFromConnectionString(connectionString);


            _device.OpenAsync().Wait();

            Console.WriteLine("Device is connected!");
             UpdateTwin(_device).Wait();

        }

        private static async Task UpdateTwin(DeviceClient device)
        {
            var twinProperties = new TwinCollection();
            twinProperties["connection.type"] = "wi-fi";
            twinProperties["connectionStrength"] = "full";

            await device.UpdateReportedPropertiesAsync(twinProperties);
        }

        public async Task SendMessage(Telemetry telemetry)
        {

            var payload = JsonConvert.SerializeObject(telemetry);

            var message = new Message(Encoding.ASCII.GetBytes(payload));

            await _device.SendEventAsync(message);
            Console.WriteLine("Message sent!");

        }

        public async Task SendMessage(IEnumerable<Telemetry> telemetries)
        {

            foreach (var telemetry in telemetries)
            {

                var payload = JsonConvert.SerializeObject(telemetry);

                var message = new Message(Encoding.ASCII.GetBytes(payload));

                await _device.SendEventAsync(message);
            }
          
            Console.WriteLine("Message sent!");

        }

    }
}
