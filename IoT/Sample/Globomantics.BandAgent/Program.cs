using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace Globomantics.BandAgent
{
    class Program
    {
        private  const string DeviceConnectionString  = @"HostName=monadDemoHub.azure-devices.net;DeviceId=monadDevice1;SharedAccessKey=pW2yJuybRVOEmUsqGjbY/HI+iSLff2k5bDZdE/uatY8=";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing Band Agent");
            var device = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
            await device.OpenAsync();
            Console.WriteLine("Device is connected!");
            // var count = 1 ;
            var twinProperties = new TwinCollection();
            twinProperties["connectionType"] = "wi-fi";
            twinProperties["connectionStrength"] = "weak";
            await device.UpdateReportedPropertiesAsync(twinProperties);
             
            // while(true) 
            // {
            //     var telemetry = new Telemetry
            //     {
            //         Message = "Dummy Object", StatusCode = count++
            //     };
            //     var serailizedData = JsonConvert.SerializeObject(telemetry);

            //     var message = new Message(Encoding.ASCII.GetBytes(serailizedData));
            //     await device.SendEventAsync(message);
            //     Thread.Sleep(2000);
            // }

            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();

        }
    }
}
