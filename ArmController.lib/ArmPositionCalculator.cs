using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib
{
    public class ArmPositionCalculator
    {
        public static ArmPositionCalculator SharedInstacne = new ArmPositionCalculator();

        public double GearRatio = 4.5;
        public int MotorStepsPerRev = 200;
        public int MicroStepSetting = 16;

        public double StepsPerRev => GearRatio * MotorStepsPerRev * MicroStepSetting;

        public double StepsPerMM => 1;

        public double AngelPerStep => 360.0 / StepsPerRev;

        public double B1 = 43;

        private ArmPositionCalculator()
        {

        }

        public Tuple<double, double, double> ToCoordinate(PosePosition pos)
        {
            return null;
        }

        public PosePosition ToPose(Tuple<double, double, double> coor)
        {
            return null;
        }

        public int AngleToMM(double a)
        {
            return (int)Math.Round(a / AngelPerStep / StepsPerMM, MidpointRounding.AwayFromZero);
        }

        public double MmToAngle(int x)
        {
            return x * StepsPerMM * AngelPerStep;
        }

        public double AngleToRadian(double angle)
        {
            return Math.Round(angle * Math.PI / 180, 5, MidpointRounding.AwayFromZero);
        }

        public double RadianToAngle(double radian)
        {
            return Math.Round(radian * 180 / Math.PI, 5, MidpointRounding.AwayFromZero);
        }

        //public Tuple<double, double, double> AngleToCoordinate(Tuple<double, double, double> angles)
        //{

        //}

        //public Tuple<double, double, double> CoordinateToAngle(Tuple<double, double, double> angles)
        //{

        //}

        public double HighAngle(int x, int y)
        {
            return 69 + MmToAngle(x) - MmToAngle(y);
        }

        public double LowAngle(int x)
        {
            return 43 + MmToAngle(x);
        }

        
    }
}
