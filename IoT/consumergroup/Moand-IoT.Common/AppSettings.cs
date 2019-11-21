namespace Moand_IoT.Common
{
    public class AppSettings
    {
        public string IoTHubName { get; set; }
        public string IoTHubConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string StorageContainerName { get; set; }
        public string ConsumerGroupName { get; set; }

        public string AnotherIoTHubName { get; set; }
        public string AnotherIoTHubConnectionString { get; set; }

    }
}
