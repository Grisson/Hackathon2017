using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmController.lib.Data;
using ArmController.Models.Data;

namespace ArmController.lib
{
    internal class TestTarget
    {
        public Guid Id;
        public string DeviceOperationSystem;
        public List<TouchPoint> Response;
    }
}
