using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System;

namespace VideoProcessor
{
    public static class ProcessVideoActivities
    {
        [FunctionName("A_TranscodeVideo")]
        public static async Task<string> TranscodeVideo(
            [ActivityTrigger] string inputVideo,
            ILogger log)
        {
            log.LogInformation($"Transcoding {inputVideo}");
            // simulate doing the activity
            await Task.Delay(5000);

            return  $"{Path.GetFileNameWithoutExtension(inputVideo)}-transcoded.mp4";
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
