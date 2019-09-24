using System;

namespace Messaging.Console
{
    public class AppSettings
    { 
        public string BusConnectionString { get; set; }
        public string QueueName { get; set; }
        public string TopicName { get; set; }
        public string TopicSubscriptionName { get; set; }

        

        
    }
}
