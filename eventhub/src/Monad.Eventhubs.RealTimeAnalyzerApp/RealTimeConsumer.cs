using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System;
using Monad.Eventhubs.Core;
using Newtonsoft.Json;
using System.Linq;

namespace Monad.Eventhubs.RealTimeAnalyzerApp
{
    public class RealTimeConsumer : IEventProcessor
    {
        private Stopwatch _checkpointStopWatch;
        private readonly List<DeviceTelemetry> _cache = new List<DeviceTelemetry>();

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Releasing lease on partion {context.Lease.PartitionId}  for reason {reason}");

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

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            throw new NotImplementedException();
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {

            var telemetryData = messages.Select(x =>
            {
                var data = Encoding.UTF8.GetString(x.Body.Array);
                return JsonConvert.DeserializeObject<DeviceTelemetry>(data);
            }).ToList();

            _cache.AddRange(telemetryData);

            var switchedOnDeviceCount = _cache.Count(x => x.IsOn);
            Console.WriteLine($"Out of {_cache.Count}, Total number of ON devices are {switchedOnDeviceCount}");

            if (_checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                _checkpointStopWatch.Restart();
            }
        }
    }
}
