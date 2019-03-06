using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Monad.Eventhubs.Core
{
    public class Consumer : IEventProcessor
    {
        private Stopwatch _checkpointStopWatch;

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
            foreach (var message in messages)
            {
                var data = Encoding.UTF8.GetString(message.Body.Array);
                Console.WriteLine($"Message received. GroupName {context.ConsumerGroupName} Partition: {context.Lease.PartitionId}, Data {data}");
            }

            if (_checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                _checkpointStopWatch.Restart();
            }
        }
    }
}
