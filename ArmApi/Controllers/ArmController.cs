using ArmApi.logic;
using ArmController.Models.Data;
using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;
using System.Web.Http;

namespace ArmApi.Controllers
{
    //[RoutePrefix("api")]
    //[ServiceRequestActionFilter]
    //public class ArmController : ApiController
    //{
        //[Route("arm/register")]
        //[HttpGet]
        //public async Task<long> Register()
        //{
        //    var id = ActorId.CreateRandom();
        //    var actor = ActorFactory.GetArm(id);
        //    await actor.RegisterAgent(id.GetLongId().ToString());
        //    return id.GetLongId();
        //}

        //[Route("arm/{id:long}/reportpose/{timeStamp}/{x:int}/{y:int}/{z:int}")]
        //[HttpPut]
        //public async Task<bool> ReportPose(long id, string timeStamp, int x, int y, int z)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    return await armActor.ReportPoseAsync(timeStamp, x, y, z);
        //    //return "ReportPose";
        //}

        //[Route("arm/{id:long}/reporttouch/{timeStamp}/{x:double}/{y:double}")]
        //[HttpPut]
        //public async Task<bool> ReportTouch(long id, string timeStamp, double x, double y)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    return await armActor.ReportTouchAsync(timeStamp, x, y);
        //    //return "ReportTouch";
        //}

        //[Route("arm/{id:long}/canresume")]
        //[HttpGet]
        //public async Task<string> CanResume(long id)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    return await armActor.CanResumeAsync();
        //}

        //[Route("arm/{id:long}/done/{data}")]
        //[HttpGet]
        //public async void Done(long id, string data)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    await armActor.DoneAsync(data);
        //}

        //[Route("arm/{id:long}/waitprob")]
        //[HttpGet]
        //public async Task<bool> WaitProb(long id)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    var result = await armActor.WaitingProbResultAsync();

        //    return result;
        //}

        //[Route("arm/{id:long}/startcalibrate")]
        //[HttpGet]
        //public async Task<string> StartCalibrate(long id)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    return await armActor.StartCalibrateAsync();
        //    //return "Calibrate commands";
        //}

        //[Route("arm/{id:long}/prob/{retry:int}")]
        //[HttpGet]
        //public async Task<string> Prob(long id, int retry = 0)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    return await armActor.ProbAsync(retry);
        //    //return "Calibrate commands";
        //}

        //[Route("arm/{id:long}/getnexttask")]
        //[HttpGet]
        //public async Task<string> GetNextTask(long id, int retry = 0)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    return await armActor.GetNextTaskAsync();
        //}

        //[Route("arm/{id:long}/coordinate/{x:double}/{y:double}/{z:double}")]
        //[HttpGet]
        //public async Task<IHttpActionResult> ConvertCoordinateToPose(long id, double x, double y, double z)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    var result = await armActor.ConvertCoordinatToPositionAsync(x, y, z);
        //    return Ok<PosePosition>(result);
        //}

        //[Route("arm/{id:long}/pose/{x:int}/{y:int}/{z:int}")]
        //[HttpGet]
        //public async Task<IHttpActionResult> ConvertPoseToCoordinate(long id, int x, int y, int z)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    var result = await armActor.ConvertPositionToCoordinatAsync(x, y, z);
        //    return Ok<double[]>(new[] { result.Item1, result.Item2, result.Item3 });
        //}

        //[Route("arm/{id:long}/touchpoint/{x:double}/{y:double}")]
        //[HttpGet]
        //public async Task<IHttpActionResult> ConvertTouchPointToPose(long id, double x, double y)
        //{
        //    var armActor = ActorFactory.GetArm(id);
        //    var result = await armActor.ConvertTouchPointToPoseAsync(x, y);
        //    return Ok<PosePosition>(result);
        //}

    //    [Route("arm/{id:long}/newtask/{taskName:string}")]
    //    [HttpPost]
    //    public void AddNextTask(long id, string taskName)
    //    {
    //        var armActor = ActorFactory.GetArm(id);
    //        armActor.AddNextTaskAsync(taskName);
    //    }
    //}
}
