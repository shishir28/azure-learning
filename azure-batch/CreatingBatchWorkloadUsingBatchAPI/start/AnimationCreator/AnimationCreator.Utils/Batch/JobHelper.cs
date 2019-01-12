using Microsoft.Azure.Batch;
using AnimationCreator.Utils.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;

namespace AnimationCreator.Utils.Bacth
{
    public class JobHelper
    {
        private string _batchAccountUri;
        private string _batchAccountName;
        private string _batchAccountKey;

        private BatchUtils _batchUtils;

        public JobHelper(string batchAccountUri, string batchAccountName, string batchAccountKey)
        {
            _batchAccountUri = batchAccountUri;
            _batchAccountName = batchAccountName;
            _batchAccountKey = batchAccountKey;
            _batchUtils = new BatchUtils(batchAccountUri, batchAccountName, batchAccountKey);
        }
        public void CreateRendererPool(string poolId) =>
            _batchUtils.CreatePoolIfNotExistAsync(poolId).Wait();
        public void CreateJob(string poolId, string jobId) =>
            _batchUtils.CreateJobAsync(jobId, poolId).Wait();

        public void AddTasks(string jobId, int taskCounts, AppSettings appSettings)
        {
            var storageConnectionString = appSettings.StorageConnectionString;
            var frameBlobContainerName = appSettings.FrameBlobContainerName;
            var sceneBlobContainerName = appSettings.SceneBlobContainerName;

            var frameBlobHelper = new BlobHelper(storageConnectionString, frameBlobContainerName, 1);
            var sceneBlobHelper = new BlobHelper(storageConnectionString, sceneBlobContainerName, 10);

            var outputContainerSasUrl = frameBlobHelper.GetContainerSasUrl(SharedAccessBlobPermissions.Write);
            
            var sceneResourceFiles = new List<ResourceFile>();
            for (int frameIndex = 0; frameIndex < taskCounts; frameIndex++)
            {
                string blobName = $"F{ frameIndex.ToString().PadLeft(6, '0') }.pi";
                var blobName1= $"F{frameIndex.ToString().PadLeft(6, '0')}.pi";
                var sceneResourceFile = sceneBlobHelper.GetResourceFile(blobName);
                sceneResourceFiles.Add(sceneResourceFile);
            }
            var tasks = _batchUtils.AddTasksAsync(jobId, sceneResourceFiles, outputContainerSasUrl).Result;
        }
    }
}