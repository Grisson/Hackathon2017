using ArmActor.Interfaces;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ArmActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class VisionActor : Actor, IVisionActor
    {
        protected static readonly string Key = "running_status";
        private static readonly string key = "cd079d2a5dca4e0d9d7224f2871e6e14";
        private static readonly string apiRoot = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";


        protected Dictionary<string, VisionAssertion> AssertionDictionary = new Dictionary<string, VisionAssertion>
        {
            ["test1"] = new VisionAssertion
            {
                CropArea = new System.Drawing.Rectangle(275, 146, 303, 104),
                TargetText = "WE ARE"
            }
        };


        /// <summary>
        /// Initializes a new instance of ArmActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public VisionActor(ActorService actorService, ActorId actorId)
                    : base(actorService, actorId)
        {
            //var data = new VisionDataModel();
            //data.AssertionDictionary = new System.Collections.Generic.Dictionary<string, VisionAssertion>
            //{ ["test"] = new VisionAssertion { CropArea = new System.Drawing.Rectangle(100, 100, 200, 200), TargetText = "" } };
        }

        public async Task<bool> Anaylyze(string containerName, string fileName, string command)
        {
            var imgData = DownloadImageToBytes(containerName, fileName);

            if (AssertionDictionary.ContainsKey(command))
            {
                var vc = AssertionDictionary[command];
                var image = BytesToBitmap(imgData);
                var aoi = cropImage(image, vc.CropArea);
                var byteData = ImageToByteArray(aoi);

                //var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=brainvision;AccountKey=13GuBE4FbGi/EBaXvTHrMFTStXnBS/VidHbVZqecGFbB5s55E+62RvndVmMd2VBF84pjIy7DR0FrrXYvSDrL9Q==;EndpointSuffix=core.windows.net");
                //var blobClient = account.CreateCloudBlobClient();
                //var container = blobClient.GetContainerReference(containerName);
                //var corpfileName = $"{command}-{fileName}";
                //var blockBlob = container.GetBlockBlobReference(corpfileName);
                //blockBlob.UploadFromByteArray(byteData, 0, byteData.Length);


                //var img2 = DownloadImageToBytes(containerName, corpfileName);
                var ocrResult = await CallOcr(byteData);

            }
            else
            {
                var ocrResult = await CallOcr(imgData);
            }

            return true;
        }

        protected byte[] DownloadImageToBytes(string containerName, string fileName)
        {
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=brainvision;AccountKey=13GuBE4FbGi/EBaXvTHrMFTStXnBS/VidHbVZqecGFbB5s55E+62RvndVmMd2VBF84pjIy7DR0FrrXYvSDrL9Q==;EndpointSuffix=core.windows.net");
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlobReference(fileName);
            blob.FetchAttributes();
            var byteData = new byte[blob.Properties.Length];
            blob.DownloadToByteArray(byteData, 0);

            return byteData;
        }

        protected async Task<OcrResults> CallOcr(byte[] imgData)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            // Request parameters and URI
            string requestParameters = "language=unk&detectOrientation =true";
            // NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + requestParameters;

            using (var content = new ByteArrayContent(imgData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(uri, content);
                var myobject = await response.Content.ReadAsStringAsync();

                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                var recognizeResult = JsonConvert.DeserializeObject<OcrResults>(myobject, settings);

                return recognizeResult;
            }
        }

        public static byte[] ImageToByteArray(Bitmap img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        protected byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        private static Bitmap cropImage(Bitmap img, System.Drawing.Rectangle cropArea)
        {
            Bitmap target = new Bitmap(cropArea.Width, cropArea.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(img, new System.Drawing.Rectangle(0, 0, target.Width, target.Height),
                                 cropArea,
                                 GraphicsUnit.Pixel);

            }
            return target; //  img.Clone(cropArea, img.PixelFormat);
        }

        private static Bitmap BytesToBitmap(byte[] imageData)
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(imageData))
            {
                bmp = new Bitmap(ms);
            }

            return bmp;
        }

        protected async Task<VisionDataModel> ReadDataAsync()
        {
            var result = await StateManager.TryGetStateAsync<VisionDataModel>(Key);
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                var tr = new VisionDataModel();
                await SaveDataAsync(tr);
                return tr;
            }
        }

        protected Task SaveDataAsync(VisionDataModel tr)
        {
            return this.StateManager.SetStateAsync<VisionDataModel>(Key, tr);
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
