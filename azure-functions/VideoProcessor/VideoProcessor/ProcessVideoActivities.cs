using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Linq;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;


using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;

namespace VideoProcessor
{
    public static class ProcessVideoActivities
    {

        [FunctionName("A_GetTranscodeBitrates")]
        public static int[] GetTranscodeBitrates(
                            [ActivityTrigger] object input,
                            ILogger log)
        {
            var bitRates = Environment.GetEnvironmentVariable("TranscodeBitrates");

            return bitRates
                        .Split(',')
                        .Select(x => Convert.ToInt32(x))
                        .ToArray();
        }

        [FunctionName("A_TranscodeVideo")]
        public static async Task<VideoFileInfo> TranscodeVideo(
            [ActivityTrigger] VideoFileInfo inputVideo,
            ILogger log)
        {

            log.LogInformation($"Transcoding {inputVideo.Location} to {inputVideo.BitRate}");

            // simulate doing the activity
            await Task.Delay(5000);

            var transcodedLocation = $"{Path.GetFileNameWithoutExtension(inputVideo.Location)}-" +
               $"{inputVideo.BitRate}kbps.mp4";

            return new VideoFileInfo
            {
                Location = transcodedLocation,
                BitRate = inputVideo.BitRate
            };
        }


        [FunctionName("A_ExtractThumbnail")]
        public static async Task<string> ExtractThumbnail(
           [ActivityTrigger] string inputVideo,
           ILogger log)
        {
            log.LogInformation($"Extracting Thumbnail {inputVideo}");

            if (inputVideo.Contains("error"))
                throw new InvalidOperationException("could not extract thumbnail");

            // simulate doing the activity
            await Task.Delay(5000);

            return "thumbnail.png";
        }

        [FunctionName("A_PrependIntro")]
        public static async Task<string> PrependIntro(
          [ActivityTrigger] string inputVideo,
          ILogger log)
        {
            log.LogInformation($"Appending Intro to {inputVideo}");
            // simulate doing the activity
            await Task.Delay(5000);
            return "withIntro.mp4";
        }



        [FunctionName("A_PublishVideo")]
        public static async Task PublishVideo(
         [ActivityTrigger] string inputVideo,
         ILogger log)
        {
            log.LogInformation($"Publishing {inputVideo}");
            // simulate doing the activity
            await Task.Delay(1000);
        }


        [FunctionName("A_RejectVideo")]
        public static async Task RejectVideo(
        [ActivityTrigger] string inputVideo,
        ILogger log)
        {
            log.LogInformation($"Rejecting {inputVideo}");
            // simulate doing the activity
            await Task.Delay(1000);
        }


        [FunctionName("A_PeriodicActivity")]
        public static void PeriodicActivity(
        [ActivityTrigger] int timesRun,
        ILogger log)
        {
            log.LogWarning($"Running the Periodic activity, run {timesRun} times ");
            // simulate doing the activity
        }


        [FunctionName("A_SendApprovalRequestEmail")]
        public static async Task SendApprovalRequestEmail(
           [ActivityTrigger] ApprovalInfo approvalInfo,
           [Table("Approvals", Connection = "AzureWebJobsStorage")] IAsyncCollector<ApprovalTableEntity> approvalTable,
           ILogger log)
        {
            var approvalCode = Guid.NewGuid().ToString("N");
            var approval = new ApprovalTableEntity
            {
                PartitionKey = "Approval",
                RowKey = approvalCode,
                OrchestrationId = approvalInfo.OrchestrationId
            };


            await approvalTable.AddAsync(approval);


            var approverEmail = new EmailAddress(Environment.GetEnvironmentVariable("ApproverEmail"));
            var senderEmail = new EmailAddress(Environment.GetEnvironmentVariable("SenderEmail"));
            var host = new EmailAddress(Environment.GetEnvironmentVariable("Host"));
            var sendGridKey = Environment.GetEnvironmentVariable("SendGridKey");


            var subject = "A video is awaiting approval";

            log.LogInformation($"Sending approval request for {approvalInfo.VideoLocation}");

            var functionAddress = $"{host}/api/SubmitVideoApproval/{approvalCode}";
            var approvedLink = functionAddress + "?result=Approved";
            var rejectedLink = functionAddress + "?result=Rejected";
            var body = $"Please review {approvalInfo.VideoLocation}<br>"
                               + $"<a href=\"{approvedLink}\">Approve</a><br>"
                               + $"<a href=\"{rejectedLink}\">Reject</a>";
            var contents = new List<Content> { new Content("text/html", body) };

            var message = new SendGridMessage { From = senderEmail, ReplyTo = approverEmail, Subject = subject, Contents = contents };

            var emailClient = new SendGrid.SendGridClient(sendGridKey);
            await emailClient.SendEmailAsync(message);

            log.LogInformation(body);
        }

        [FunctionName("A_Cleanup")]
        public static async Task<string> Cleanup(
        [ActivityTrigger] string[] filesToCleanup,
        ILogger log)
        {
            foreach (var file in filesToCleanup)
            {

                log.LogInformation($"Deleting {file}");
                // simulate doing the activity
                await Task.Delay(5000);
            }
            return "Cleaned Up Successfully";
        }

    }
}
