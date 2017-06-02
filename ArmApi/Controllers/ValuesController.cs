using System.Collections.Generic;
using System.Web.Http;

namespace ArmApi.Controllers
{
    [RoutePrefix("api")]
    [ServiceRequestActionFilter]
    public class ValuesController : ApiController
    {
        // GET api/values 
        [Route("values")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
