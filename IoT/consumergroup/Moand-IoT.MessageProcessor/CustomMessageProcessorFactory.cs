using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Moand_IoT.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moand_IoT.MessageProcessor
{

    public class CustomMessageProcessorFactory : IEventProcessorFactory
    {
        public CustomMessageProcessorFactory(AppSettings appSettings) =>
            this._appSettings = appSettings;
        private AppSettings _appSettings;

        IEventProcessor IEventProcessorFactory.CreateEventProcessor(PartitionContext context) =>
            new CustomMessageProcessor(_appSettings);
    }
}