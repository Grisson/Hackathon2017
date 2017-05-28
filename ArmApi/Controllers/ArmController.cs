using ArmController.Models.Data;
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
        public IEnumerable<string> Register()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("arm/{id}/reporttouch/{timeStamp}/{x:double}/{y:double}")]
        [HttpPut]
        public string ReportTouch(string id, string timeStamp, double x, double y)
        {
            return "ReportTouch";
        }

        [Route("arm/{id}/reportpose/{timeStamp}/{x:int}/{y:int}/{z:int}")]
        [HttpPut]
        public string ReportPose(string id, string timeStamp, int x, int y, int z)
        {
            return "ReportPose";
        }

        [Route("arm/{id}/startcalibrate")]
        [HttpGet]
        public string StartCalibrate(string id)
        {
            return "Calibrate commands";
        }

        [Route("arm/{id}/EndCalibrate")]
        [HttpGet]
        public string EndCalibrate(string id)
        {
            return "Calibrated";
        }

        [Route("arm/{id}/topose/{x:double}/{y:double}")]
        [HttpGet]
        public IHttpActionResult ConvertToPose(string id, double x, double y)
        {
            return Ok<PosePosition>(new PosePosition(1,2,3));
        }


        [Route("arm/{id}/touch/{x:double}/{y:double}")]
        [HttpGet]
        public IHttpActionResult Touch(string id, double x, double y)
        {
            return Ok<PosePosition>(new PosePosition(1, 2, 3));
        }
    }
}
