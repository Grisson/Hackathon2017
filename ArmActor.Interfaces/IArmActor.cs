using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using ArmController.Models.Data;
using ArmController.Models.Command;

namespace ArmActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IArmActor : IActor
    {
        Task<bool> ReportTouch(string timeStamp, double x, double y);

        Task<bool> ReportPose(string timeStamp, int x, int y, int z);

        Task<string> StartCalibrate();

        Task<bool> EndCalibrate();

        Task<PosePosition> ConvertToPose(double x, double y);

        Task<IEnumerable<BaseCommand>> Touch(double x, double y);

        ///// <summary>
        ///// TODO: Replace with your own actor method.
        ///// </summary>
        ///// <returns></returns>
        //Task<int> GetCountAsync(CancellationToken cancellationToken);

        ///// <summary>
        ///// TODO: Replace with your own actor method.
        ///// </summary>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //Task SetCountAsync(int count, CancellationToken cancellationToken);
    }
}
