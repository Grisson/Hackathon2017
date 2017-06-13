using ArmController.Models.Data;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Threading.Tasks;

namespace ArmActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IArmActor : IActor
    {
        Task<bool> RegisterAgent(string agentId);

        Task<bool> ReportPoseAsync(string timeStamp, int x, int y, int z);

        Task<bool> ReportTouchAsync(string timeStamp, double x, double y);

        Task<string> CanResumeAsync();

        Task DoneAsync(string data);

        Task<bool> WaitingProbResultAsync();

        Task<string> StartCalibrateAsync();

        Task<string> ProbAsync(int retry);

        Task<PosePosition> ConvertCoordinatToPositionAsync(double x, double y, double z);

        Task<Tuple<double, double, double>> ConvertPositionToCoordinatAsync(int x, int y, int z);

        Task<PosePosition> ConvertTouchPointToPoseAsync(double x, double y);

    }
}
