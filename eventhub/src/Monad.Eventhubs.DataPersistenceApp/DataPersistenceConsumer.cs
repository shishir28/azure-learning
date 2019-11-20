using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;
using Monad.Eventhubs.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Monad.Eventhubs.DataPersistenceApp
{
    public class DataPersistenceConsumer : IEventProcessor
    {
        private string _databaseConnectionString;

        private Stopwatch _checkpointStopWatch;
        private readonly List<DeviceTelemetry> _cache = new List<DeviceTelemetry>();

        public DataPersistenceConsumer(string databaseConnectionString)
        {
            this._databaseConnectionString = databaseConnectionString;
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Releasing lease on Partition {context.Lease.PartitionId}  for reason {reason}");

            if (reason == CloseReason.Shutdown)
                await context.CheckpointAsync();

            Console.ReadLine();
        }

        public async Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"I am listening to Partition {context.Lease.PartitionId}, at position {context.Lease.Offset}.");
            _checkpointStopWatch = new Stopwatch();
            _checkpointStopWatch.Start();
            await Task.FromResult<object>(null);
        }

        public async Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Message {error.Message}");
            Console.WriteLine($"StackTrace {error.StackTrace}");
            await Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Console.WriteLine($"..........................New Batch arrived...........................");

            var telemetryDataCollection = messages.Select(x =>
            {
                var data = Encoding.UTF8.GetString(x.Body.Array);
                return JsonConvert.DeserializeObject<DeviceTelemetry>(data);
            }).ToList();


            using (var sqlConnection = new SqlConnection(this._databaseConnectionString))
            {
                await sqlConnection.OpenAsync();
                var query = @"INSERT INTO [DeviceTelemetry] ([IPAddress],[Time],[DeviceType],[IsOn]) VALUES (@IPAddress, @Time, @DeviceType, @IsOn)";

                foreach (DeviceTelemetry tetelemetryData in telemetryDataCollection)
                {
                    Console.WriteLine($"{tetelemetryData.IpAddress} Time: { tetelemetryData.Time} and DeviceType {tetelemetryData.DeviceType} IsOn { tetelemetryData.IsOn}");

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@IPAddress", tetelemetryData.IpAddress);
                        sqlCommand.Parameters.AddWithValue("@Time", tetelemetryData.Time);
                        sqlCommand.Parameters.AddWithValue("@DeviceType", tetelemetryData.DeviceType);
                        sqlCommand.Parameters.AddWithValue("@IsOn", tetelemetryData.IsOn);
                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }

            if (_checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(1))
            {
                await context.CheckpointAsync();
                _checkpointStopWatch.Restart();
            }
        }
    }
}