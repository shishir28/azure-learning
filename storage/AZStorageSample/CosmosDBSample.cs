using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AZStorageSample

{
    public class EmployeeEntity
    {
        public EmployeeEntity()
        {
        }
        public EmployeeEntity(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString() =>
          JsonConvert.SerializeObject(this);

    }

    public class CosmosDBSample
    {
        public static void Invoke(AppSettings appSettings)
        {
            var documentClient = new DocumentClient(new Uri(appSettings.CosmosDBEndpoint), appSettings.CosmosDBKey);
            InsertData(documentClient, "John", "Doe").GetAwaiter().GetResult();
            InsertData(documentClient, "David", "Smith").GetAwaiter().GetResult();
            InsertData(documentClient, "Roger", "Smith").GetAwaiter().GetResult();
            InsertData(documentClient, "Harry", "Callahan").GetAwaiter().GetResult();
            ReadDataByLastName(documentClient, "Smith");

        }
        private static async Task InsertData(DocumentClient documentClient, string firstName, string lastName)
        {
            var databaseName = "maindb";
            var collectionName = "employee";
            var employeeInstance = new EmployeeEntity { FirstName = firstName, LastName = lastName };

            await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
           employeeInstance);

        }

        private static void ReadDataByLastName(DocumentClient documentClient, string lastName)
        {
            var databaseName = "maindb";
            var collectionName = "employee";
            var feedOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            var queryResult = documentClient.CreateDocumentQuery<EmployeeEntity>(
               UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), feedOptions
            ).Where(x => x.LastName == lastName);

            Console.WriteLine("Listing Result...");
            foreach (var employee in queryResult)
                Console.WriteLine(employee);

        }

    }
}
