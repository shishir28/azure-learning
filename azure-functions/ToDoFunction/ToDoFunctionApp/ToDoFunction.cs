using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Linq;

namespace ToDoFunctionApp
{
    public static class TodoApi
    {
        public static IList<ToDo> items = new List<ToDo>();

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            TraceWriter log)
        {
            log.Info("Creating a new Todo");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<ToDoCreateModel>(requestBody);
            var todo = new ToDo() { TaskDescription = input.TaskDescription };
            items.Add(todo);
            return new OkObjectResult(todo);
        }


        [FunctionName("GetTodos")]
        public static IActionResult GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            TraceWriter log)
        {
            log.Info("Getting a list of Todos");
            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
            TraceWriter log, string id)
        {
            log.Info("Getting Todo with matching id");
            var item = items.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return new NotFoundResult();

            return new OkObjectResult(item);
        }


        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
            TraceWriter log, string id)
        {
            log.Info("Updating existing Todo");

            var item = items.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return new NotFoundResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<ToDoUpdateModel>(requestBody);
            item.IsCompleted = input.IsCompleted;

            if (!string.IsNullOrWhiteSpace(input.TaskDescription))
                item.TaskDescription = input.TaskDescription;

            return new OkObjectResult(item);
        }


        [FunctionName("DeleteTodo")]
        public static IActionResult DeleteTodo(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
           TraceWriter log, string id)
        {
            log.Info("Deleting existing Todo");

            var item = items.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return new NotFoundResult();

            items.Remove(item);

            return new OkResult();
        }
    }
}
