using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messaging.Console
{
    class Program
    {
        static void Main (string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 100;
            var services = new ServiceCollection ();

            var builder = new ConfigurationBuilder ()
                .SetBasePath (Directory.GetCurrentDirectory ())
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true);

            IConfigurationRoot configuration = builder.Build ();

            services.Configure<AppSettings> (configuration.GetSection ("AppSettings"));
            var serviceProvider = services.BuildServiceProvider ();

            var options = serviceProvider.GetRequiredService<IOptions<AppSettings>> ();

            var appsettings = options.Value;

            System.Console.WriteLine ("Please Select one of the following  option by pressing , 1, 2 , 3 or 4  ");
            System.Console.WriteLine ("1. Send a message to Queue");
            System.Console.WriteLine ("2. Read a message from Queue");
            System.Console.WriteLine ("3. Send a message to Topic");
            System.Console.WriteLine ("4. Receive a message from Topic");
            System.Console.WriteLine ("5. Terminate the Application");
            var keySelected = System.Console.ReadKey ().KeyChar;
            switch (keySelected)
            {
                case '1':
                    SendMessageToQueue (appsettings).GetAwaiter ().GetResult ();
                    break;

                case '2':
                    ReadMessageFromQueue (appsettings);
                    break;
                case '3':
                    SendMessageToTopic (appsettings).GetAwaiter ().GetResult ();;
                    break;

                case '4':
                    ReceiveMessageFromTopic (appsettings);;
                    break;
                default:
                    ExitTheApplication (appsettings);
                    break;

            }
            System.Console.WriteLine ("Press any key to terminate");
            System.Console.ReadKey ();
        }

        private static async Task SendMessageToQueue (AppSettings appSettings)
        {
            System.Console.WriteLine ("Type Message  you wish to send..");
            var queueClient = new QueueClient (appSettings.BusConnectionString, appSettings.QueueName);
            var messageBody = System.Console.ReadLine ();
            var message = new Message (Encoding.UTF8.GetBytes (messageBody));
            System.Console.WriteLine ($"Sending message: {messageBody}");
            await queueClient.SendAsync (message);
        }

        private static void ReadMessageFromQueue (AppSettings appSettings)
        {
            System.Console.WriteLine ("Message from queue is ");
            var queueClient = new QueueClient (appSettings.BusConnectionString, appSettings.QueueName);
            var messageHandlerOptions = new MessageHandlerOptions (ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler ((msg, token) =>
                {
                    System.Console.WriteLine ($"Received message: SequenceNumber:{msg.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(msg.Body)}");
                    return queueClient.CompleteAsync (msg.SystemProperties.LockToken);
                },
                messageHandlerOptions);
            var queueName = System.Console.ReadLine ();
        }

        private static async Task SendMessageToTopic (AppSettings appSettings)
        {
            System.Console.WriteLine ("Type Message  you wish to publish to queue..");
            var topicClient = new TopicClient (appSettings.BusConnectionString, appSettings.TopicName);
            var messageBody = System.Console.ReadLine ();
            var message = new Message (Encoding.UTF8.GetBytes (messageBody));
            System.Console.WriteLine ($"Sending message: {messageBody}");
            await topicClient.SendAsync (message);
        }

        private static void ReceiveMessageFromTopic (AppSettings appSettings)
        {
            System.Console.WriteLine ("Message from Topic is ");
            var subscriptionClient = new SubscriptionClient (appSettings.BusConnectionString, appSettings.TopicName, appSettings.TopicSubscriptionName);
            var messageHandlerOptions = new MessageHandlerOptions (ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            subscriptionClient.RegisterMessageHandler ((msg, token) =>
                {
                    System.Console.WriteLine ($"Received message: SequenceNumber:{msg.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(msg.Body)}");
                    return subscriptionClient.CompleteAsync (msg.SystemProperties.LockToken);
                },
                messageHandlerOptions);
            var queueName = System.Console.ReadLine ();
        }
        private static void ExitTheApplication (AppSettings appSettings)
        {
            System.Console.WriteLine ("Going to terminate the Application...");
        }

        static Task ExceptionReceivedHandler (ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            System.Console.WriteLine ($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            System.Console.WriteLine ("Exception context for troubleshooting:");
            System.Console.WriteLine ($"- Endpoint: {context.Endpoint}");
            System.Console.WriteLine ($"- Entity Path: {context.EntityPath}");
            System.Console.WriteLine ($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }

}