using ArmController.Models.Data;
using System;
using System.Collections.Generic;

namespace ArmController.lib
{
    internal class TestTarget
    {
        public Guid Id;
        public string DeviceOperationSystem;
        public List<TouchPoint> Response;
    }
}
