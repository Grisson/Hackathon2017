using ArmActor.Interfaces;
using ArmController.lib;
using ArmController.Models.Data;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Threading.Tasks;

namespace ArmActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class ArmActor : Actor, IArmActor
    {
        protected static readonly string Key = "running_status";

        /// <summary>
        /// Initializes a new instance of ArmActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ArmActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<bool> RegisterAgent(string agentId)
        {
            var tr = await ReadDataAsync();
            tr.RegisterTestAgent(agentId);
            await SaveDataAsync(tr);
            return true;
        }

        public async Task<bool> ReportPoseAsync(string timeStamp, int x, int y, int z)
        {
            var tr = await ReadDataAsync();
            tr.ReportAgentPosePosition(long.Parse(timeStamp), x, y, z);
            await SaveDataAsync(tr);
            return true;
        }

        public async Task<bool> ReportTouchAsync(string timeStamp, double x, double y)
        {
            var tr = await ReadDataAsync();
            tr.ReportTouchBegin(long.Parse(timeStamp), x, y);
            await SaveDataAsync(tr);
            return true;
        }

        public async Task<string> CanResumeAsync()
        {
            var tr = await ReadDataAsync();
            var result = tr.CanResume();
            await SaveDataAsync(tr);
            return result;
        }

        public async Task DoneAsync(string data)
        {
            var tr = await ReadDataAsync();
            tr.Done(data);
            await SaveDataAsync(tr);
        }

        public async Task<bool> WaitingProbResultAsync()
        {
            var tr = await ReadDataAsync();
            
            return tr.WaitingProbResult();
        }

        public async Task<string> StartCalibrateAsync()
        {
            var tr = await ReadDataAsync();
            var result = tr.GetSecondCalibrationCommonds();
            await SaveDataAsync(tr);
            return result;
        }

        public async Task<string> ProbAsync(int retry)
        {
            var tr = await ReadDataAsync();
            var result = tr.GetProbCommands(retry);
            await SaveDataAsync(tr);
            return result;
        }
        
        public async Task<string> GetNextTaskAsync()
        {
            var tr = await ReadDataAsync();
            var result = tr.GetNextTask();
            await SaveDataAsync(tr);
            return result;
        }

        public async Task<PosePosition> ConvertCoordinatToPositionAsync(double x, double y, double z)
        {
            var tr = await ReadDataAsync();
            var result = tr.ConvertCoordinatToPosition(new Tuple<double, double, double>(x, y, z));
            await SaveDataAsync(tr);
            return result;
        }

        public async Task<Tuple<double, double, double>> ConvertPositionToCoordinatAsync(int x, int y, int z)
        {
            var tr = await ReadDataAsync();
            var result = tr.ConvertPositionToCoordinat(new PosePosition(x, y, z));
            await SaveDataAsync(tr);
            return result;
        }

        public async Task<PosePosition> ConvertTouchPointToPoseAsync(double x, double y)
        {
            var tr = await ReadDataAsync();
            var result = tr.ConvertTouchPointToPosition(new TouchPoint(x, y));
            return result;
        }

        public async Task AddNextTaskAsync(string taskName)
        {
            var tr = await ReadDataAsync();
            tr.AddNextTask(taskName);
            await SaveDataAsync(tr);
        }

        protected async Task<TestRunner> ReadDataAsync()
        {
            var result = await StateManager.TryGetStateAsync<TestRunner>(Key);
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                var tr = new TestRunner();
                await SaveDataAsync(tr);
                return tr;
            }
        }

        protected Task SaveDataAsync(TestRunner tr)
        {
            return this.StateManager.SetStateAsync<TestRunner>(Key, tr);
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        
    }
}
