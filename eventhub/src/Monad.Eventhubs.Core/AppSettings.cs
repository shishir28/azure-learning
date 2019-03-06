using System;

namespace Monad.Eventhubs.Core
{
    public class AppSettings
    {
        public string EventHubConnectionStrings { get; set; }
        public string PublisherConnectionStrings { get; set; }

        public string SubscriberConnectionStrings { get; set; }

        public string EventHubName { get; set; }
        public string StorageAccounName { get; set; }
        public string StorageAccountKey { get; set; }

    }
}
