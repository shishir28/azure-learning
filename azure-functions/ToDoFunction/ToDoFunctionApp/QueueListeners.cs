using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ToDoFunctionApp
{
    public static class QueueListeners
    {
        [FunctionName("QueueListeners")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("todos", Connection = "AzureWebJobsStorage")]ToDo todo,
            [Blob("todos", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log)
        {
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{todo.Id}.txt");
            await blob.UploadTextAsync($"Create a new task: {todo.TaskDescription}");
            log.LogInformation($"C# Queue trigger function processed: {todo.TaskDescription}");
        }
    }
}
