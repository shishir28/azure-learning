using System;
using System.Threading.Tasks;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.Batch.Common;

namespace AnimationCreator.Utils.Bacth
{
    public class BatchUtils
    {
        private BatchClient _batchClient;

        public BatchUtils(string bathAccountUrl, string bathAccountName, string bathAccountKey)
        {
            var keyCredentials = new BatchSharedKeyCredentials(bathAccountUrl, bathAccountName, bathAccountKey);
            _batchClient = BatchClient.Open(keyCredentials);

        }

        public async Task CreatePoolIfNotExistAsync(string poolId)
        {
            try
            {
                Console.WriteLine($"Creating pool {poolId}...");

                Func<ImageReference, bool> imageScanner = imageRef =>
                    imageRef.Publisher == "MicrosoftWindowsServer" &&
                    imageRef.Offer == "WindowsServer" &&
                    imageRef.Sku.Contains("2012-R2-Datacenter");

                var skuAndImage = await GetNodeAgentSkuReferenceAsync(imageScanner);

                var pool = _batchClient.PoolOperations.CreatePool(poolId: poolId,
                                virtualMachineSize: "standard_d1_v2", //VM Size
                                targetDedicatedComputeNodes: 1,
                                virtualMachineConfiguration: new VirtualMachineConfiguration(skuAndImage.Image, skuAndImage.SKU.Id) // VM configuration
                                );
                pool.ApplicationPackageReferences = new List<ApplicationPackageReference>
                {
                    new ApplicationPackageReference{ApplicationId ="PolyRay",Version="1"}
                };
                await pool.CommitAsync();

            }
            catch (BatchException ex)
            {
                if (ex.RequestInformation?.BatchError != null && ex.RequestInformation.BatchError.Code == BatchErrorCodeStrings.PoolExists)
                {
                    Console.WriteLine($"The Pool {poolId} already existed so could not create it again!");
                }
                else
                {
                    throw;

                }
            }
        }

        public async Task CreateJobAsync(string jobId, string poolId)
        {
            Console.WriteLine($"Creating job {jobId} ...");
            var job = _batchClient.JobOperations.CreateJob();
            job.Id = jobId;
            job.PoolInformation = new PoolInformation { PoolId = poolId };
            job.OnAllTasksComplete = OnAllTasksComplete.TerminateJob;
            await job.CommitAsync();
        }

        public async Task<List<CloudTask>> AddTasksAsync(string jobId, List<ResourceFile> sceneFiles, string outputContainerSasUrl)
        {
            Console.WriteLine($"Adding { sceneFiles.Count} tasks to job {jobId}...");
            var tasks = new List<CloudTask>();
            foreach (var sceneFile in sceneFiles)
            {
                var taskId = "task" + sceneFiles.IndexOf(sceneFile).ToString().PadLeft(6, '0');
                var sceneFileName = sceneFile.FilePath;
                var imageFileName = sceneFileName.Replace(".pi", ".tga");
                string taskCommandLine = $"cmd /c %AZ_BATCH_APP_PACKAGE_POLYRAY%\\polyray.exe {sceneFileName} -o {imageFileName}";
                var task = new CloudTask(taskId, taskCommandLine);
                task.ResourceFiles = new List<ResourceFile> { sceneFile };
                task.OutputFiles = new List<OutputFile>
                {
                    new OutputFile (
                        filePattern:imageFileName,
                        destination:new OutputFileDestination(new OutputFileBlobContainerDestination(containerUrl:outputContainerSasUrl, path:taskId +$@"\{imageFileName}")),
                        uploadOptions:new OutputFileUploadOptions(OutputFileUploadCondition.TaskCompletion)
                    )
                };
                tasks.Add(task);

            }
            await _batchClient.JobOperations.AddTaskAsync(jobId, tasks);
            return tasks;

        }

        private async Task<SkuAndImage> GetNodeAgentSkuReferenceAsync(Func<ImageReference, bool> scanFunc)
        {
            var nodeAgentSkus = await _batchClient.PoolOperations.ListNodeAgentSkus().ToListAsync();
            var nodeAgentSku = nodeAgentSkus.First(x => x.VerifiedImageReferences.FirstOrDefault(scanFunc) != null);
            var imageReference = nodeAgentSku.VerifiedImageReferences.First(scanFunc);
            return new SkuAndImage(nodeAgentSku, imageReference);
        }
    }
}