using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

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
            try
            {
                transcodedLocation = await ctx.CallActivityAsync<string>("A_TranscodeVideo", videoLocation);


                if (!ctx.IsReplaying)
                    log.LogInformation("Calling Thumbnail Extraction activity");

                thumbnailLocation = await ctx.CallActivityAsync<string>("A_ExtractThumbnail", transcodedLocation);

                if (!ctx.IsReplaying)
                    log.LogInformation("Calling Prepending Intro Video  generation activity");


                withIntroductionLocation = await ctx.CallActivityAsync<string>("A_PrependIntro", transcodedLocation);

                return new
                {
                    Transcoded = transcodedLocation,
                    Thumbnail = thumbnailLocation,
                    WithIntro = withIntroductionLocation
                };

            }
            catch (Exception ex)
            {
                if (!ctx.IsReplaying)
                    log.LogInformation($"Caught an error from activity {ex.Message}");

                await ctx.CallActivityAsync<string>("A_Cleanup",
                   new[]{ transcodedLocation,
                     thumbnailLocation,
                     withIntroductionLocation }
                    );
                return new
                {
                    Error = "Failed to process upload video",
                    Message = ex.Message
                };
            }



        }
    }
}
