using Microsoft.Azure.Batch;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using System.Threading;
using System;

namespace AnimationCreator.Utils.Storage
{
    public class BlobHelper
    {
        private int _maxParallelThreads;
        private CloudBlobContainer _blobContainer;
        private static Semaphore _semaphore;

        public BlobHelper(string storageConenctionString, string blobContainerName, int maxParallelThreads)
        {
            _maxParallelThreads = maxParallelThreads;
            var blobClient = CloudStorageAccount.Parse(storageConenctionString).CreateCloudBlobClient();
            _blobContainer = blobClient.GetContainerReference(blobContainerName);
            _blobContainer.CreateIfNotExistsAsync().Wait();
            _semaphore = new Semaphore(maxParallelThreads, maxParallelThreads);

        }

        public void UploadText(string blobName, string content)
        {
            var blob = _blobContainer.GetBlockBlobReference(blobName);
            _semaphore.WaitOne();
            var blobTask = blob.UploadTextAsync(content).ContinueWith(x =>
            {
                _semaphore.Release();
                Console.Write("-->");
            });
        }

        public void UploadFile(string filePath)
        {
            var blobName = Path.GetExtension(filePath);
            _blobContainer.GetBlockBlobReference(blobName).UploadFromFileAsync(filePath).Wait();
        }

        public ResourceFile GetResourceFile(string blobName)
        {
            var blob = _blobContainer.GetBlockBlobReference(blobName);
            var sasContrainsts = new SharedAccessBlobPolicy
            {

                SharedAccessExpiryTime = DateTime.UtcNow.AddDays(7),
                Permissions = SharedAccessBlobPermissions.Read
            };
            var sasToken = blob.GetSharedAccessSignature(sasContrainsts);
            var blobSasUri = $"{blob.Uri}{sasToken}";
            return ResourceFile.FromUrl(blobSasUri, blobName);
        }

        public string GetContainerSasUrl(SharedAccessBlobPermissions permissions)
        {
            var sasContrainsts = new SharedAccessBlobPolicy
            {

                SharedAccessExpiryTime = DateTime.UtcNow.AddDays(7),
                Permissions = permissions
            };
            var sasToken = _blobContainer.GetSharedAccessSignature(sasContrainsts);
            return $"{_blobContainer.Uri}{sasToken}";
        }

        public void DownloadFrames(string folderName, int count)
        {
            ;
            for (int frameIndex = 0; frameIndex < count; frameIndex++)
            {
                var blobName = $"task{ frameIndex.ToString().PadLeft(6, '0') }/{ GetFrameName(frameIndex) }";
                var blob = _blobContainer.GetBlockBlobReference(blobName);
                _semaphore.WaitOne();

                blob.DownloadToFileAsync(Path.Combine(folderName, GetFrameName(frameIndex)), FileMode.Create).ContinueWith(x =>
                {
                    _semaphore.WaitOne();
                    Console.Write("=>");
                });
            }
        }

        private string GetFrameName(int frameIndex)
             
        {
        
            return $"F{frameIndex.ToString().PadLeft(6, '0') }.tga";
        }

    }
}