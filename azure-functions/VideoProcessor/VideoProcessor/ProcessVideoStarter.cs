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

        [FunctionName("SubmitVideoApproval")]
        public static async Task<HttpResponseMessage> SubmitVideoApproval(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SubmitVideoApproval/{id}")]
           HttpRequestMessage req,
           [OrchestrationClient] DurableOrchestrationClient client,
           [Table("Approvals", "Approval", "{id}", Connection = "AzureWebJobsStorage")] ApprovalTableEntity approval,
           ILogger log)
        {
            // nb if the approval code doesn't exist, framework just returns a 404 before we get here

            string result = req.RequestUri.ParseQueryString()["result"];

            if (result == null)
                return req.CreateResponse(HttpStatusCode.BadRequest, "Need an approval result");

            log.LogWarning($"Sending approval result to {approval.OrchestrationId} of {result}");
            // send the ApprovalResult external event to this orchestration
            await client.RaiseEventAsync(approval.OrchestrationId, "ApprovalResult", result);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
