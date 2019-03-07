using Microsoft.Azure.EventHubs.Processor;

namespace Monad.Eventhubs.DataPersistenceApp
{
    public class DataPersistenceConsumerFactory : IEventProcessorFactory
    {
        public DataPersistenceConsumerFactory(string databaseConnectionString) =>
            this._databaseConnectionString = databaseConnectionString;
        private string _databaseConnectionString;

        IEventProcessor IEventProcessorFactory.CreateEventProcessor(PartitionContext context) =>
            new DataPersistenceConsumer(_databaseConnectionString);
    }
}