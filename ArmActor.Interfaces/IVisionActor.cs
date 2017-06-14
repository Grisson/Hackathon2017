using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmActor.Interfaces
{
    public interface IVisionActor : IActor
    {
        Task<VisionAssertionResult> Anaylyze(string containerName, string fileName, string command);
    }
}
