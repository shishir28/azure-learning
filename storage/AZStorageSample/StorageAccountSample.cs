using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;
using System;

namespace AZStorageSample
{
    public class StorageAccountSample
    {
        public static void Invoke(string connectionStrings)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionStrings);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("objective2");
            // UploadBlob(container);
            // ListAttributes(container);
            // SetMetadata(container);
            //  ListMetadata(container);
            // CopyBlob(container);
            // UploadBlobSubDirectory(container);
            // CreateSharedAccessPolicy(container);
        }

        private static async void UploadBlob(CloudBlobContainer container)
        {
            await container.CreateIfNotExistsAsync();
            var blockBlob = container.GetBlockBlobReference("examobjectives");
            using (var filestream = System.IO.File.OpenRead(@"C:\Users\SXM29\Downloads\532_OD_Changes.pdf"))
            {
                await blockBlob.UploadFromStreamAsync(filestream);
            }
        }

        private static async void ListAttributes(CloudBlobContainer container)
        {
            await container.FetchAttributesAsync();
            Console.WriteLine($"Container Name {container.StorageUri.PrimaryUri.ToString()}");
            Console.WriteLine($"Last Modified Date {container.Properties.LastModified.ToString()}");
        }
      
        private static async void SetMetadata(CloudBlobContainer container)
        {
            Console.WriteLine($"Setting New Metadata...");

            container.Metadata.Clear();
            container.Metadata["Author"] = "Shishir Kumar Mishra";
            container.Metadata["AuthorOn"] = DateTime.Now.ToLongTimeString();
            await container.SetMetadataAsync();
        }
       
        private static async void ListMetadata(CloudBlobContainer container)
        {
            Console.WriteLine($"Printing Existing  Metadata...");
            await container.FetchAttributesAsync();
            foreach (var item in container.Metadata)
            {
                Console.WriteLine($"Key {item.Key}");
                Console.WriteLine(item.Value);
            }
        }

        private static async void CopyBlob(CloudBlobContainer container)
        {
            var blockBlob = container.GetBlockBlobReference("examobjectives");
            var copyBlockBlob = container.GetBlockBlobReference("examobjectives-copy");
            await copyBlockBlob.StartCopyAsync(new Uri(blockBlob.Uri.AbsoluteUri));
        }

        private static async void UploadBlobSubDirectory(CloudBlobContainer container)
        {
            var parentDirectoryReference = container.GetDirectoryReference("parent-directory");
            var childDirectoryReference = parentDirectoryReference.GetDirectoryReference("child-directory");
            var blockBlob = childDirectoryReference.GetBlockBlobReference("new-examobjectives");

            using (var filestream = System.IO.File.OpenRead(@"C:\Users\SXM29\Downloads\532_OD_Changes.pdf"))
            {
                await blockBlob.UploadFromStreamAsync(filestream);
            }
        }

        private static async void CreateSharedAccessPolicy(CloudBlobContainer container)
        {
            Console.WriteLine("Inside CreateSharedAccessPolicy");
            var sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
            };
            var permissions = new BlobContainerPermissions();
            permissions.SharedAccessPolicies.Clear();
            permissions.SharedAccessPolicies.Add("policy", sharedPolicy);
            await container.SetPermissionsAsync(permissions);
        }

        private static async void CreateCORSPolicy(CloudBlobClient blobClient)
        {
            var serviceProperties = new ServiceProperties();
            serviceProperties.Cors.CorsRules.Add(new CorsRule
            {
                AllowedMethods = CorsHttpMethods.Get | CorsHttpMethods.Post,
                AllowedHeaders = new List<string>() { "http://localhost:8080" },
                MaxAgeInSeconds = 3600

            });
            await blobClient.SetServicePropertiesAsync(serviceProperties);
        }

    }
}
