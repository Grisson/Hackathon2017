﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Models.Data
{
    public class TouchPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public long TouchTimeStamp { get; set; }


        public TouchPoint()
        {
            TouchTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public TouchPoint(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

    }
}
