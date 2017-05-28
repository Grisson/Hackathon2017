﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib
{
    public static class ExtensionMethod
    {
        public static double RandWithFiveDigites(this double a)
        {
            return Math.Round(a, 5, MidpointRounding.AwayFromZero);
        }

        public static bool IsEqualWithInTolerance(this double a, double b, double tolerance)
        {
            if (Math.Abs(a - b) < tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
