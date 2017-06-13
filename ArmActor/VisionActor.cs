using System;
using System.Threading.Tasks;
using ArmActor.Interfaces;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors;
using Microsoft.WindowsAzure.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ProjectOxford.Vision;
using Microsoft.WindowsAzure.Storage.Blob;
using ArmController.lib;
using System.IO;

namespace ArmActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class VisionActor : Actor, IVisionActor
    {
        protected static readonly string Key = "running_status";
        private static readonly string key = "cd079d2a5dca4e0d9d7224f2871e6e14";
        private static readonly string apiRoot = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";

        /// <summary>
        /// Initializes a new instance of ArmActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public VisionActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<bool> Anaylyze(string containerName, string fileName, string command)
        {

            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=brainvision;AccountKey=13GuBE4FbGi/EBaXvTHrMFTStXnBS/VidHbVZqecGFbB5s55E+62RvndVmMd2VBF84pjIy7DR0FrrXYvSDrL9Q==;EndpointSuffix=core.windows.net");
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlobReference(fileName);
            //blob.FetchAttributes();
            //var byteData = new byte[blob.Properties.Length];
            blob.DownloadToFile(fileName, FileMode.CreateNew);

            var client = new HttpClient();

            // Request headers. Replace the example key with a valid subscription key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            // Request parameters and URI
            string requestParameters = "language=unk&detectOrientation =true";

            // NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + requestParameters;
            var byteData = GetImageAsByteArray(fileName);
            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(uri, content);
                var myobject = await response.Content.ReadAsStringAsync();

                Console.WriteLine(myobject);

            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
           
            return false;
        }


        byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        protected async Task<TestRunner> ReadDataAsync()
        {
            var result = await StateManager.TryGetStateAsync<TestRunner>(Key);
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                var tr = new TestRunner();
                await SaveDataAsync(tr);
                return tr;
            }
        }

        protected Task SaveDataAsync(TestRunner tr)
        {
            return this.StateManager.SetStateAsync<TestRunner>(Key, tr);
        }


        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }
    }
}
