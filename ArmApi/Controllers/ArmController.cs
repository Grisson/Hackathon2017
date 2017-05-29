using ArmApi.logic;
using ArmController.Models.Data;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ArmApi.Controllers
{
    [RoutePrefix("api")]
    [ServiceRequestActionFilter]
    public class ArmController : ApiController
    {
        [Route("arm/register")]
        [HttpGet]
        public async Task<long> Register()
        {
            var id = ActorId.CreateRandom();
            var actor = ActorFactory.GetArm(id);
            await actor.RegisterAgent(id.GetLongId().ToString());
            return id.GetLongId();
        }

        [Route("arm/{id:long}/reporttouch/{timeStamp}/{x:double}/{y:double}")]
        [HttpPut]
        public async Task<bool> ReportTouch(long id, string timeStamp, double x, double y)
        {
            var armActor = ActorFactory.GetArm(id);
            return await armActor.ReportTouchAsync(timeStamp, x, y);
            //return "ReportTouch";
        }

        [Route("arm/{id:long}/reportpose/{timeStamp}/{x:int}/{y:int}/{z:int}")]
        [HttpPut]
        public async Task<bool> ReportPose(long id, string timeStamp, int x, int y, int z)
        {
            var armActor = ActorFactory.GetArm(id);
            return await armActor.ReportPoseAsync(timeStamp, x, y, z);
            //return "ReportPose";
        }

        [Route("arm/{id:long}/startcalibrate")]
        [HttpGet]
        public string StartCalibrate(long id)
        {
            return "Calibrate commands";
        }

        [Route("arm/{id:long}/EndCalibrate")]
        [HttpGet]
        public string EndCalibrate(long id)
        {
            return "Calibrated";
        }

        [Route("arm/{id:long}/topose/{x:double}/{y:double}")]
        [HttpGet]
        public IHttpActionResult ConvertToPose(long id, double x, double y)
        {
            return Ok<PosePosition>(new PosePosition(1,2,3));
        }


        [Route("arm/{id:long}/touch/{x:double}/{y:double}")]
        [HttpGet]
        public IHttpActionResult Touch(long id, double x, double y)
        {
            return Ok<PosePosition>(new PosePosition(1, 2, 3));
        }
    }
}
