using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamsa.Azure
{
    public partial class Cognitive
    {
        public async Task<Face[]> DetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    return await DetectFaces(imageFileStream);
                }
            }
            catch (Exception)
            {
                return new Face[0];
            }
        }

        public async Task<Face[]> DetectFaces(Bitmap image)
        {
            var fileName = $"{DateTime.Now.Ticks}.jpg";
            image.Save(fileName);

            Face[] result = null;
            using (Stream s = File.OpenRead(fileName))
            {
                result = await DetectFaces(s);
            }

            if(File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            return result;
        }


        public async Task<Face[]> DetectFaces(Stream imageStream)
        {
            if (FaceServiceClient == null)
            {
                FaceServiceClient = new FaceServiceClient(FaceSecretKey, FaceApiBaseUrl);
            }

            Face[] result = new Face[0];
            try
            {
                result = await FaceServiceClient.DetectAsync(imageStream, true, true);
            }
            catch (Exception)
            {
                // log
                //return new FaceRectangle[0];
            }

            return result;
        }
    }
}
