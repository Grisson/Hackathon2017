using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ArmApi.Controllers
{
    [RoutePrefix("api/target/{id:long}")]
    [ServiceRequestActionFilter]
    public class TestTargetController : ApiController
    {
        [Route("reporttouch/{timeStamp}/{x:double}/{y:double}")]
        [HttpPut]
        public string ReportTouch(long id, string timeStamp, double x, double y)
        {
            return "ReportTouch";
        }
    }
}
