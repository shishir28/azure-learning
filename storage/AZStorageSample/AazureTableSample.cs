using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using System;

namespace AZStorageSample

{
    public class CarEntity : TableEntity
    {
        public CarEntity()
        {
        }
        public CarEntity(int Id, int year, string make, string model, string color)
        {
            this.PartitionKey = "car";
            this.RowKey = Id.ToString();
            this.UniqueId = Id;
            this.Year = year;
            this.Make = make;
            this.Model = model;
            this.Color = color;
        }
        public int UniqueId { get; set; }
        public int Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }

    }
    public class AazureTableSample
    {
        public static void Invoke(string connectionStrings)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionStrings);
            // PerformTableWork(storageAccount);
            //PerformQueueWork(storageAccount);         
        }
        private static async void PerformTableWork(CloudStorageAccount storageAccount)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var tableReference = tableClient.GetTableReference("FirstTestTable");
            await tableReference.CreateIfNotExistsAsync();
            var cars = new CarEntity[]{ new CarEntity(11, 2001, "Toyota", "Camry", "Blue"),
                new CarEntity(12, 2002, "Toyota", "Camry", "White"),
                new CarEntity(13, 2003, "Honda", "Civic", "Black"),
                new CarEntity(14, 2004, "Toyota", "Rav4", "Red"),
                new CarEntity(55, 2005, "Audi", "A5", "Black"),
            };

            TableBatchOperation tbo = new TableBatchOperation();
            foreach (var car in cars)
            {

                //     var opertaion = TableOperation.Insert(car);
                //    await  tableReference.ExecuteAsync(opertaion);
                tbo.Insert(car);

            }

            await tableReference.ExecuteBatchAsync(tbo);

            var query = new TableQuery<CarEntity>();
            foreach (var car in await tableReference.ExecuteQuerySegmentedAsync<CarEntity>(query, token: null))
            {
                Console.WriteLine("==================================");
                Console.WriteLine($"Make {car.Make}  Model {car.Model} Color {car.Model} Year {car.Year}");
            }
        }

        private static async void PerformQueueWork(CloudStorageAccount storageAccount)
        {
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queueReference = queueClient.GetQueueReference("thismessagequeue");
            await queueReference.CreateIfNotExistsAsync();

            var message = new CloudQueueMessage("Hey theere 3!");
            await queueReference.AddMessageAsync(message);
            var oldMessage = await queueReference.PeekMessageAsync(); //GetMessage
            Console.WriteLine(oldMessage.AsString);
        }

    }
}
