using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Hamsa.Azure
{
    public class OCRResult
    {
        public double TextAngle { get; set; }

        public string Orientation { get; set; }

        public string Language { get; set; }

        public List<OCRRegions> Regions { get; set; }
    }

    public class OCRRegions
    {
        public string BoundingBox { get; set; }

        public List<OCRLine> Lines { get; set; }
    }

    public class OCRLine
    {
        public string BoundingBox { get; set; }

        public List<OCRWord> Words { get; set; }
    }

    public class OCRWord
    {
        public string BoundingBox { get; set; }
        public string Text { get; set; }
    }

    public static class StringExtension
    {
        public static Tuple<int, int, int, int> ParseBoundingBox(this string rawData)
        {
            if(string.IsNullOrEmpty(rawData))
            {
                return null;
            }

            var lists = rawData.Trim().Split(',');

            if(lists.Length != 4)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new Tuple<int, int, int, int> (int.Parse(lists[0]), int.Parse(lists[1]), int.Parse(lists[2]), int.Parse(lists[3]));
        }
    }

    public partial class Cognitive
    {
        public OCRResult OCR(byte[] imageBytes)
        {
            var client = new HttpClient();

            // Request headers. NOTE: Replace this example key with a valid subscription key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", VisionSecretKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "visualFeatures=Categories&language=en";

            // NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            string uri = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?" + requestParameters;


            //HttpResponseMessage response;

            // Request body. Try this sample with a locally stored JPEG image.
            OCRResult result = null;
            using (var content = new ByteArrayContent(imageBytes))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //response = client.PostAsync(uri, content).Result;

                var task = client.PostAsync(uri, content).ContinueWith((taskwithresponse) =>
                 {
                     var tmp = taskwithresponse.Result;
                     var jsonString = tmp.Content.ReadAsStringAsync();
                     jsonString.Wait();
                     result = JsonConvert.DeserializeObject<OCRResult>(jsonString.Result);

                 });
                task.Wait();
            }

            return result;
        }

        public OCRResult OCR(Bitmap image)
        {
            ImageConverter converter = new ImageConverter();
            var bytes = (byte[])converter.ConvertTo(image, typeof(byte[]));

            return OCR(bytes);
        }
    }
}
