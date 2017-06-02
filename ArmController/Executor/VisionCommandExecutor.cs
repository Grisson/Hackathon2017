using ArmController.Models.Command;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types

namespace ArmController.Executor
{
    public class VisionCommandExecutor : IExecutor
    {
        public static readonly VisionCommandExecutor SharedInstance = new VisionCommandExecutor();

        private CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private CloudBlobClient blobClient;
        private CloudBlobContainer container;
        public Action<string> LogHandler => CommandExecutor.SharedInstance.LogHandler;
        public Action<string> TakePhoto => CommandExecutor.SharedInstance.TakePhoto;

        private VisionCommandExecutor()
        {
            blobClient = storageAccount.CreateCloudBlobClient();
            var registerId = "test"; ;
            if(CommandExecutor.SharedInstance.RegisterId.HasValue)
            {
                registerId = CommandExecutor.SharedInstance.RegisterId.Value.ToString();
            }
            container = blobClient.GetContainerReference($"{registerId}-image");
            container.CreateIfNotExists();
        }

        public void Execute(BaseCommand command)
        {
            this.Execute(command as VisionCommand);
        }

        public void Execute(VisionCommand command)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            TakePhoto(fileName);

            Thread.Sleep(1000);

            if (File.Exists(fileName))
            {
                //SendImageToServer(fileName, command.Data);
                UploadImageAsBlob(fileName).Wait();
            }

            lock (CommandExecutor.SharedInstance)
            {
                CommandStore.SharedInstance.CurrentCommand = null;
                CommandExecutor.SharedInstance.IsWaitingResponse = false;
            }
        }

        byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        async void SendImageToServer(string imageFilePath, string data)
        {
            var client = new HttpClient();
            var registerId = CommandExecutor.SharedInstance.RegisterId;
            string uri = $"{ConfigurationManager.AppSettings["BaseUrl"]}/api/vision/{registerId}/{data}";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
            }

            File.Delete(imageFilePath);
        }

        async Task UploadImageAsBlob(string imageFilePath)
        {
            var blockBlob = container.GetBlockBlobReference(imageFilePath);
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            //blockBlob.Properties.ContentType = image.ContentType;

            await blockBlob.UploadFromByteArrayAsync(byteData, 0, byteData.Length);
        }
    }
}


/*
 * CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
    CloudConfigurationManager.GetSetting("StorageConnectionString"));

// Create the blob client.
CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

// Retrieve reference to a previously created container.
CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

// Retrieve reference to a blob named "photo1.jpg".
CloudBlockBlob blockBlob = container.GetBlockBlobReference("photo1.jpg");

// Save blob contents to a file.
using (var fileStream = System.IO.File.OpenWrite(@"path\myfile"))
{
    blockBlob.DownloadToStream(fileStream);
}

    // Retrieve storage account from connection string.
CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
    CloudConfigurationManager.GetSetting("StorageConnectionString"));

// Create the blob client.
CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

// Retrieve reference to a previously created container.
CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

// Retrieve reference to a blob named "myblob.txt".
CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob.txt");

// Delete the blob.
blockBlob.Delete();
 */
