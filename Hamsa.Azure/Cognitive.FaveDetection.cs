using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Drawing;
using System.IO;

namespace Hamsa.Azure
{
    public partial class Cognitive
    {
        public Face[] DetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    return DetectFaces(imageFileStream);
                }
            }
            catch (Exception)
            {
                return new Face[0];
            }
        }

        public Face[] DetectFaces(Bitmap image)
        {
            var fileName = $"{DateTime.Now.Ticks}.jpg";
            image.Save(fileName);

            Face[] result = null;
            using (Stream s = File.OpenRead(fileName))
            {
                result = DetectFaces(s);
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            return result;
        }


        public Face[] DetectFaces(Stream imageStream)
        {
            if (FaceServiceClient == null)
            {
                FaceServiceClient = new FaceServiceClient(FaceSecretKey, FaceApiBaseUrl);
            }

            Face[] result = new Face[0];
            try
            {
                result = FaceServiceClient.DetectAsync(imageStream, true, true).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // log
                //return new FaceRectangle[0];
            }

            return result;
        }
    }
}
