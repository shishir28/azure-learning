using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Linq;

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
