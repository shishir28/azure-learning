using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

            var transcodedLocation = await ctx.CallActivityAsync<string>("A_TranscodeVideo", videoLocation);


            if (!ctx.IsReplaying)
                log.LogInformation("Calling Thumbnail Extraction activity");

            var thumbnailLocation = await ctx.CallActivityAsync<string>("A_ExtractThumbnail", transcodedLocation);

            if (!ctx.IsReplaying)
                log.LogInformation("Calling Prepending Intro Video  generation activity");


            var withIntroductionLocation = await ctx.CallActivityAsync<string>("A_PrependIntro", transcodedLocation);

            return new
            {
                Transcoded = transcodedLocation,
                Thumbnail = thumbnailLocation,
                WithIntro = withIntroductionLocation
            };

        }
    }
}
