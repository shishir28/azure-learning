using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Monad.Eventhubs.Core
{
    public class Publisher
    {
        private EventHubClient _eventHubClient;

        public void Init(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
        }

        public async Task PublishAsync<T>(T myEvent)
        {
            if (myEvent == null) throw new ArgumentNullException(nameof(myEvent));
            // 1.Serialize the event
            var searialzedEvent = JsonConvert.SerializeObject(myEvent);
            // 2.Equals Get encoded bytes
            var eventBytes = Encoding.UTF8.GetBytes(searialzedEvent);

            // 3.Push  it to EventData
            var eventData = new EventData(eventBytes);

            // 4 Publish it to bus
            await _eventHubClient.SendAsync(eventData);

        }

        public async Task PublishAsync<T>(IEnumerable<T> myEvents)
        {
            if (myEvents == null) throw new ArgumentNullException(nameof(myEvents));

            var events = new List<EventData>();
            foreach (var myEvent in myEvents)
            {
                var searialzedEvent = JsonConvert.SerializeObject(myEvent);
                var eventBytes = Encoding.UTF8.GetBytes(searialzedEvent);
                var eventData = new EventData(eventBytes);
                events.Add(eventData);
            }
            await _eventHubClient.SendAsync(events);
        }

    }
}
