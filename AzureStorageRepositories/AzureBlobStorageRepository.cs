using Common.Interfaces;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;


namespace VariableBase.Mathematics
{
    public class AzureBlobStorageRepository: IStorageRepository
    {
        String StorageConnectionString = "Add connection string here";

        public void Create(String containerName, String blobName, String value)
        {

            CloudStorageAccount account = CloudStorageAccount.Parse(this.StorageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            
            var container = serviceClient.GetContainerReference(containerName.ToLower());
            container.CreateIfNotExistsAsync().Wait();
            
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName.ToLower());
            blob.UploadTextAsync(value).Wait();
        }

        public String Get(String containerName, String blobName)
        {
            String result = String.Empty;

            CloudStorageAccount account = CloudStorageAccount.Parse(this.StorageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            
            var container = serviceClient.GetContainerReference(containerName.ToLower());
            container.CreateIfNotExistsAsync().Wait();

            using (var sm = new MemoryStream())
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName.ToLower());
                blob.DownloadToStreamAsync(sm).Wait();
                sm.Seek(0, SeekOrigin.Begin);
                using (var sw = new StreamReader(sm))
                {
                    result = sw.ReadToEnd();
                }
            }

            return result;
        }
    }
}
