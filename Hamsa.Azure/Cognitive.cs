using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamsa.Azure
{
    public partial class Cognitive
    {
        public string VisionSecretKey { get; private set; }
        public string FaceSecretKey { get; set; }
        public string FaceApiBaseUrl { get; set; }

        public FaceServiceClient FaceServiceClient;

        public Cognitive()
        {
            VisionSecretKey = ConfigurationManager.AppSettings["VisionAPIKey"];
            FaceSecretKey = ConfigurationManager.AppSettings["FaceAPIKey"];
            FaceApiBaseUrl = ConfigurationManager.AppSettings["FaceAPIUrl"];
            FaceServiceClient = new FaceServiceClient(FaceSecretKey);
        }

        public Cognitive(string secretKey)
        {
            VisionSecretKey = secretKey;
        }

        
    }
}
