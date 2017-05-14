﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib
{
    public class GeometryHelper
    {
        public static Tuple<double, double> CalculateGrad(double ax, double ay, double az)
        {
            double rrot = Math.Sqrt((ax * ax) + (ay * ay));
            double rside = Math.Sqrt((rrot * rrot) + (az * az));

            double rot = Math.Asin(ax / rrot);
            double high = Math.Acos((rside * 0.5) / 120) * 2;

            double low = 0;

            if(az > 0)
            {
                low = Math.Asin(rrot / rside) + ((Math.PI - high) / 2.0) - (Math.PI / 2.0);

            }
            else
            {
                low = Math.PI - Math.Asin(rrot / rside) + ((Math.PI - high) / 2.0) - (Math.PI / 2.0);
            }

            high = high + low;

            return new Tuple<double, double>(high, low);


        }
    }
}