using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ArmApi.Controllers
{
    [RoutePrefix("api/vision/{id:long}")]
    [ServiceRequestActionFilter]
    public class VisionController : ApiController
    {
        //[Route("vision/{id:long}/upload/{command:string}")]
        [Route("upload/{command}")]
        [HttpPost]
        public async Task<IHttpActionResult> Upload(long id, string command)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();
                //Do whatever you want with filename and its binaray data.
            }

            return Ok();
        }

        [Route("analyze/{filename}/{command}")]
        [HttpPost]
        public async Task<IHttpActionResult> Analyze(long id, string filename, string command)
        {
            //if (!Request.Content.IsMimeMultipartContent())
            //    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            //var provider = new MultipartMemoryStreamProvider();
            //await Request.Content.ReadAsMultipartAsync(provider);
            //foreach (var file in provider.Contents)
            //{
            //    var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
            //    var buffer = await file.ReadAsByteArrayAsync();
            //    //Do whatever you want with filename and its binaray data.
            //}
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=brainvision;AccountKey=13GuBE4FbGi/EBaXvTHrMFTStXnBS/VidHbVZqecGFbB5s55E+62RvndVmMd2VBF84pjIy7DR0FrrXYvSDrL9Q==;EndpointSuffix=core.windows.net");
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference($"{Math.Abs(id)}-image");
            var blob = container.GetBlobReference(filename);
            MemoryStream memStream = new MemoryStream();
            blob.DownloadToStream(memStream);
            //blob.DownloadToByteArray
            blob.DownloadToFile("TestImage.jpg", FileMode.CreateNew);


            return Ok();
        }
    }
}
