using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VideoProcessor
{
    public static class ProcessVideoOrchestrators
    {
        [FunctionName("O_ProcessVideo")]
        public static async Task<object> ProcessVideo(
            [OrchestrationTrigger] DurableOrchestrationContext ctx,
            ILogger log)
        {
            var videoLocation = ctx.GetInput<string>();

            if (!ctx.IsReplaying)
                log.LogInformation("Calling Transcoding activity");

            string transcodedLocation = null;
            string thumbnailLocation = null;
            string withIntroductionLocation = null;
            string approvalResult = "Unknown";

            try
            {

                var transCodeResult = await ctx.CallSubOrchestratorAsync<VideoFileInfo[]>("O_TranscodeVideo", videoLocation);

                transcodedLocation = transCodeResult.OrderByDescending(x => x.BitRate).Select(y => y.Location).FirstOrDefault();


                if (!ctx.IsReplaying)
                    log.LogInformation("Calling Thumbnail Extraction activity");

                thumbnailLocation = await ctx.CallActivityAsync<string>("A_ExtractThumbnail", transcodedLocation);

                if (!ctx.IsReplaying)
                    log.LogInformation("Calling Prepending Intro Video  generation activity");


                withIntroductionLocation = await ctx.CallActivityAsync<string>("A_PrependIntro", transcodedLocation);

                await ctx.CallActivityAsync("A_SendApprovalRequestEmail", new ApprovalInfo()
                {
                    OrchestrationId = ctx.InstanceId,
                    VideoLocation = withIntroductionLocation
                });


                using (var cts = new CancellationTokenSource())
                {
                    var timeout = ctx.CurrentUtcDateTime.AddMinutes(3);
                    var timeoutTask = ctx.CreateTimer(timeout, cts.Token);
                    var approvalTask = ctx.WaitForExternalEvent<string>("ApprovalResult");
                    var winner = await Task.WhenAny(approvalTask, timeoutTask);
                    if (winner == approvalTask)
                    {
                        approvalResult = approvalTask.Result;
                        cts.Cancel();
                        if (!ctx.IsReplaying)
                            log.LogInformation("ApprovalTask was winner");
                    }
                    else
                    {
                        if (!ctx.IsReplaying)
                            log.LogInformation("Timeout was winner");

                        approvalResult = "Timed Out";
                    }
                }


                if (approvalResult == "Approved")
                    await ctx.CallActivityAsync("A_PublishVideo", withIntroductionLocation);
                else
                    await ctx.CallActivityAsync("A_RejectVideo", withIntroductionLocation);

                return new
                {
                    Transcoded = transcodedLocation,
                    Thumbnail = thumbnailLocation,
                    WithIntro = withIntroductionLocation,
                    ApprovalResult = approvalResult
                };

            }
            catch (Exception ex)
            {
                if (!ctx.IsReplaying)
                    log.LogInformation($"Caught an error from activity {ex.Message}");

                await ctx.CallActivityAsync<string>("A_Cleanup",
                   new[]{ transcodedLocation,
                     thumbnailLocation,
                     withIntroductionLocation });
                return new
                {
                    Error = "Failed to process upload video",
                    Message = ex.Message
                };
            }

        }

        [FunctionName("O_TranscodeVideo")]
        public static async Task<VideoFileInfo[]> TranscodeVideo(
           [OrchestrationTrigger] DurableOrchestrationContext ctx,
           ILogger log)
        {
            var videoLocation = ctx.GetInput<string>();


            if (!ctx.IsReplaying)
                log.LogInformation("Calling  A_GetTranscodeBitrates activity");

            var bitRates = await ctx.CallActivityAsync<int[]>("A_GetTranscodeBitrates", null);


            var transcodeTasks = new List<Task<VideoFileInfo>>();

            foreach (var bitRate in bitRates)
            {
                var fileInfo = new VideoFileInfo() { BitRate = bitRate, Location = videoLocation };
                var task = ctx.CallActivityAsync<VideoFileInfo>("A_TranscodeVideo", fileInfo);
                transcodeTasks.Add(task);
            }

            var transCodeResult = await Task.WhenAll(transcodeTasks);
            return transCodeResult;
        }

        [FunctionName("O_PeriodicTask")]
        public static async Task<int> PeriodicTask(
          [OrchestrationTrigger] DurableOrchestrationContext ctx,
          ILogger log)
        {

            var timesRun = ctx.GetInput<int>();
            timesRun++;
            if (!ctx.IsReplaying)
                log.LogInformation($"Starting the PeriodicTask activity {ctx.InstanceId}, {timesRun}");
            await ctx.CallActivityAsync("A_PeriodicActivity", timesRun);
            var nextRun = ctx.CurrentUtcDateTime.AddSeconds(30);
            await ctx.CreateTimer(nextRun, CancellationToken.None);
            ctx.ContinueAsNew(timesRun);
            return timesRun;

        }
    }
}
