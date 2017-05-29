using ArmActor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace ArmApi.logic
{
    public static class ActorFactory
    {
        private static readonly Uri ArmActorUrl = new Uri("fabric:/CloudBrian/ArmActorService");

        public static IArmActor GetArm(string actorId)
        {
            return ActorProxy.Create<IArmActor>(new ActorId(actorId), ArmActorUrl);
        }

        public static IArmActor GetArm(ActorId actorId)
        {
            return ActorProxy.Create<IArmActor>(actorId, ArmActorUrl);
        }
    }
}
