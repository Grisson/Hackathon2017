using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController
{
    public static class StepperMotorHelper
    {
        public static double GearRatio = 90 / 20;
        public static double MicroSteps = 16;
        public static double StepsPerRev = 200;


        public static double mapRadiusToStep(double gearRatio, double stepsPreRev)
        {
            return gearRatio * stepsPreRev / 2 / Math.PI;
        }

    }
}
