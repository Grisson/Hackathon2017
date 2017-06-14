using ArmActor;
using ArmApi.logic;
using Microsoft.ServiceFabric.Actors;
using System;
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
        public async Task<VisionAssertionResult> Analyze(long id, string filename, string command)
        {
            //var idd = ActorId.CreateRandom();
            var actor = ActorFactory.GetVision(id);
            var result = await actor.Anaylyze($"{Math.Abs(id)}-image", filename, command);

            return result;
        }
    }
}
