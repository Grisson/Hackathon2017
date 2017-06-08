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
        public FaceServiceClient faceServiceClient;

        public FaceRectangle[] UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    return UploadAndDetectFaces(imageFileStream);
                }
            }
            catch (Exception)
            {
                return new FaceRectangle[0];
            }
        }

        public FaceRectangle[] UploadAndDetectFaces(Bitmap image)
        {
            if (faceServiceClient == null)
            {
                faceServiceClient = new FaceServiceClient(SecretKey);
            }

            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Jpeg);
                return UploadAndDetectFaces(memoryStream);
            }
        }


        public FaceRectangle[] UploadAndDetectFaces(Stream imageStream)
        {
            if (faceServiceClient == null)
            {
                faceServiceClient = new FaceServiceClient(SecretKey);
            }

            FaceRectangle[] result = new FaceRectangle[0];
            try
            {
                var task = faceServiceClient.DetectAsync(imageStream).ContinueWith((taskWithFaces) =>
                {
                    var faces = taskWithFaces.Result;
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    result = faceRects.ToArray();
                });
                task.Wait();
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
