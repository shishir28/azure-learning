using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace ToDoFunctionApp
{
    public static class TodoApi
    {

        [FunctionName ("CreateTodo")]
        public static async Task<IActionResult> CreateTodo (
            [HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req, [Table ("todos", Connection = "AzureWebJobsStorage")] IAsyncCollector<ToDoTableEntity> todoTable, [Queue ("todos", Connection = "AzureWebJobsStorage")] IAsyncCollector<ToDo> todoQueue,

            ILogger log)
        {
            log.LogInformation ("Creating a new Todo");
            string requestBody = await new StreamReader (req.Body).ReadToEndAsync ();
            var input = JsonConvert.DeserializeObject<ToDoCreateModel> (requestBody);
            var todo = new ToDo () { TaskDescription = input.TaskDescription };
            await todoTable.AddAsync (todo.ToTableEntity ());
            await todoQueue.AddAsync (todo);
            return new OkObjectResult (todo);
        }

        [FunctionName ("GetTodos")]
        public static async Task<IActionResult> GetTodos (
            [HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req, [Table ("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation ("Getting a list of Todos");
            var query = new TableQuery<ToDoTableEntity> ();
            var segment = await todoTable.ExecuteQuerySegmentedAsync (query, null);
            return new OkObjectResult (segment.Select (Mappings.ToToDo));
        }

        [FunctionName ("GetTodoById")]
        public static IActionResult GetTodoById (
            [HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req, [Table ("todos", "TODO", "{id}", Connection = "AzureWebJobsStorage")] ToDoTableEntity todo,
            ILogger log, string id)
        {
            log.LogInformation ("Getting Todo with matching id");

            if (todo == null)
                return new NotFoundResult ();

            return new OkObjectResult (todo.ToToDo ());
        }

        [FunctionName ("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo (
            [HttpTrigger (AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req, [Table ("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log, string id)
        {
            log.LogInformation ("Updating existing Todo");
            string requestBody = await new StreamReader (req.Body).ReadToEndAsync ();
            var input = JsonConvert.DeserializeObject<ToDoUpdateModel> (requestBody);
            var findOperation = TableOperation.Retrieve<ToDoTableEntity> ("TODO", id);

            var item = await todoTable.ExecuteAsync (findOperation);

            if (item.Result == null)
                return new NotFoundResult ();

            var existingItem = item.Result as ToDoTableEntity;
            existingItem.IsCompleted = input.IsCompleted;

            if (!string.IsNullOrWhiteSpace (input.TaskDescription))
                existingItem.TaskDescription = input.TaskDescription;

            var replaceOperation = TableOperation.Replace (existingItem);
            await todoTable.ExecuteAsync (replaceOperation);

            return new OkObjectResult (existingItem.ToToDo ());
        }

        [FunctionName ("DeleteTodo")]
        public static async Task<IActionResult> DeleteTodo (
            [HttpTrigger (AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req, [Table ("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log, string id)
        {
            log.LogInformation ("Deleting existing Todo");
            var deleteOperation = TableOperation.Delete (new TableEntity () { PartitionKey = "TODO", RowKey = id, ETag = "*" });

            try
            {
                var deleteResult = await todoTable.ExecuteAsync (deleteOperation);
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == 404)
            {
                return new NotFoundResult ();
            }

            return new OkResult ();
        }
    }
}