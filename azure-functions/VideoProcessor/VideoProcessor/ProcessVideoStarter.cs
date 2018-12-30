using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;


namespace VideoProcessor
{
    public static class ProcessVideoStarter
    {
        [FunctionName("ProcessVideoStarter")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClientBase starter,
            ILogger log)
        {

            string video = req.RequestUri.ParseQueryString()["video"];

            if (video == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest,
                   "Please pass the video location the query string");
            }
            log.LogInformation($"About to start orchestration for {video}");

            var orchestrationId = await starter.StartNewAsync("O_ProcessVideo", video);

            return starter.CreateCheckStatusResponse(req, orchestrationId);


        }
    }
}
