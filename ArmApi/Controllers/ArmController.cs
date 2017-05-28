using ArmController.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ArmApi.Controllers
{
    [RoutePrefix("api/arm")]
    [ServiceRequestActionFilter]
    public class ArmController : ApiController
    {
        [Route("register")]
        [HttpGet]
        public IEnumerable<string> Register()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("reporttouch/{timeStamp}/{x:double}/{y:double}")]
        [HttpPut]
        public string ReportTouch()
        {
            return "ReportTouch";
        }

        [Route("reportpose/{timeStamp}/{x:int}/{y:int}/{z:int}")]
        [HttpPut]
        public string ReportPose(string timeStamp, int x, int y, int z)
        {
            return "ReportPose";
        }

        [Route("startcalibrate")]
        [HttpGet]
        public string StartCalibrate()
        {
            return "Calibrate commands";
        }

        [Route("EndCalibrate")]
        [HttpGet]
        public string EndCalibrate()
        {
            return "Calibrated";
        }

        [Route("topose/{x:double}/{y:double}")]
        [HttpGet]
        public IHttpActionResult ConvertToPose()
        {
            return Ok<PosePosition>(new PosePosition(1,2,3));
        }
    }
}
