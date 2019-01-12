using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace ToDoFunctionApp
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, 
            [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var query = new TableQuery<ToDoTableEntity>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            var deleted = 0;

            foreach (var todo in segment)
            {
                if (todo.IsCompleted)
                {
                    await todoTable.ExecuteAsync(TableOperation.Delete(todo));
                    deleted++;
                }
            }

            log.LogInformation($"Delete {deleted} item at {DateTime.Now}");

        }
    }
}
