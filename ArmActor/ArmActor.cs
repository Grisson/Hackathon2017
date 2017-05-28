﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ArmActor.Interfaces;
using ArmController.Models.Command;
using ArmController.Models.Data;

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
        /// <summary>
        /// Initializes a new instance of ArmActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ArmActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task<PosePosition> ConvertToPose(double x, double y)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EndCalibrate()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReportPose(string timeStamp, int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReportTouch(string timeStamp, double x, double y)
        {
            throw new NotImplementedException();
        }

        public Task<string> StartCalibrate()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BaseCommand>> Touch(double x, double y)
        {
            throw new NotImplementedException();
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
