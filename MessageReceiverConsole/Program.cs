using Emgu.CV;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MessageReceiverConsole
{
    class Program
    {
        private static readonly string key = "cd079d2a5dca4e0d9d7224f2871e6e14";
        static void Main(string[] args)
        {
            Console.Write("Enter image file path: ");
            string imageFilePath = @".\test.jpg";//Console.ReadLine();

            var image = GetImageAsByteArray(imageFilePath);
            
            MakeOCRRequest(imageFilePath);

            Console.WriteLine("\n\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async void MakeOCRRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers. Replace the example key with a valid subscription key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            // Request parameters and URI
            string requestParameters = "language=unk&detectOrientation =true";

            // NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                var myobject = await response.Content.ReadAsStringAsync();

                Console.WriteLine(myobject);

            }
        }

        static async void MakeAnalysisRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers. NOTE: Replace this example key with a valid subscription key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "visualFeatures=Categories&language=en";

            // NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            string uri = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?" + requestParameters;
            Console.WriteLine(uri);

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
        }
    }
}


//Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", "ocmmobiletest", string.Empty);

//string name = "RootManageSharedAccessKey";
//string key = "wg7KwOaE9H44iqmn7MidIfZ0688NGQvZRDZ4IcKIFJs=";
//TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(name, key);

//NamespaceManager namespaceManager = new NamespaceManager(uri, tokenProvider);
//namespaceManager.CreateTopic("DataCollectionTopic");

//namespaceManager.CreateSubscription("DataCollectionTopic", "Inventory");
//namespaceManager.CreateSubscription("DataCollectionTopic", "Dashboard");

//MessagingFactory factory = MessagingFactory.Create(uri, tokenProvider);
//BrokeredMessage bm = new BrokeredMessage("xxx");
//bm.Label = "SalesReport";
//bm.Properties["StoreName"] = "Redmond";
//bm.Properties["MachineID"] = "POS_1";

//MessageSender sender = factory.CreateMessageSender("DataCollectionTopic");
//sender.Send(bm);

//MessageReceiver receiver = factory.CreateMessageReceiver("DataCollectionTopic/subscriptions/Inventory");
//BrokeredMessage receivedMessage = receiver.Receive();
//try
//{
//    //ProcessMessage(receivedMessage);
//    receivedMessage.Complete();
//}
//catch (Exception e)
//{
//    receivedMessage.Abandon();
//}