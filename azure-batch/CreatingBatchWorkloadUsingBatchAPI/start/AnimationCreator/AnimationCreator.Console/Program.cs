using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using AnimationCreator.Utils;
using AnimationCreator.Utils.Bacth;
using AnimationCreator.Utils.Storage;

namespace AnimationCreator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 100;
            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<AppSettings>>();

            var appsettings = options.Value;

            int width = 1280;
            int height = 720;

            int numberOfFrames = 10;
            string poolName = "RenderPool";
            string jobName = "RenderJob";

            // Create the Animation and upload to Blob storage
            //System.Console.WriteLine("Press enter to CreateTestAnimation...");
            //System.Console.ReadLine();
            //CreateTestAnimation(width, height, numberOfFrames, generateErrors: false, appSettings:appsettings);
            //System.Console.WriteLine("CreateTestAnimation complete!");

            // Create the Pool
            //System.Console.WriteLine("Press enter to CreatePool...");
            ////System.Console.ReadLine();
            //CreatePool(poolName, appsettings);
            //System.Console.WriteLine("CreatePool complete!");

            // Create the Azure Batch job

            System.Console.WriteLine("Press enter to CreateJob...");
            //System.Console.ReadLine();
            //CreateJob(poolName, jobName, appsettings);
            //System.Console.WriteLine("CreateJob complete!");

            //// Add tasks to the job
            //System.Console.WriteLine("Press enter to AddTasks...");
            //System.Console.ReadLine();
            //AddTasks(jobName, numberOfFrames, appsettings);
            //System.Console.WriteLine("AddTasks complete!");

            // // Download the animation frames
            System.Console.WriteLine("Press enter to DownloadFrames...");
            //System.Console.ReadLine();
            DownloadFrames(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Animation"), numberOfFrames, appsettings);
            System.Console.WriteLine("DownloadFrames complete!");

            System.Console.WriteLine("Demo Complete!");
            System.Console.ReadLine();
        }

        private static void DownloadFrames(string folderName, int count, AppSettings appSettings)
        {
            var blobHelper = new BlobHelper(appSettings.StorageConnectionString, appSettings.FrameBlobContainerName, 10);
            blobHelper.DownloadFrames(folderName, count);
        }

        private static void AddTasks(string jobId, int numberOfTasks, AppSettings appSettings)
        {
            var jobHelper = new JobHelper(appSettings.BatchAccountUrl, appSettings.BatchAccountName, appSettings.BatchAccountKey);
            jobHelper.AddTasks(jobId, numberOfTasks,appSettings);
        }

        private static void CreateJob(string poolId, string jobId, AppSettings appSettings)
        {
            var jobHelper = new JobHelper(appSettings.BatchAccountUrl, appSettings.BatchAccountName, appSettings.BatchAccountKey);
            jobHelper.CreateJob(poolId, jobId);
        }

        private static void CreatePool(string poolId, AppSettings appSettings)
        {
            var jobHelper = new JobHelper(appSettings.BatchAccountUrl, appSettings.BatchAccountName, appSettings.BatchAccountKey);
            jobHelper.CreateRendererPool(poolId);
        }

        private static void CreateTestAnimation(int width, int height, int numberOfFrames, bool generateErrors, AppSettings appSettings)
        {
            var animationCreator = new AnimationCreator(width, height);
            animationCreator.Frames = numberOfFrames;
            animationCreator.GenerateErrors = generateErrors;
            animationCreator.CreateSceneFiles(appSettings);
        }
    }
}
