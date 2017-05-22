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

        public double B1 = 43; // degree
        public double B2 = 69; // degree

        public int l = 120; // 120mm arm

        private ArmPositionCalculator()
        {

        }

        public Tuple<double, double, double> ToCoordinate(PosePosition pos)
        {
            var lowAngle = LowAngle(pos.X);
            var highAngle = HighAngle(pos.X, pos.Y);

            var lowRadian = AngleToRadian(lowAngle);
            var highRadian = AngleToRadian(highAngle);

            var alphaAngle = (180 - highAngle) / 2.0;
            var alpha = AngleToRadian(alphaAngle);

            var bottomL = BottomOfIsoscelesTriangle(l, alpha);

            var beta = (Math.PI - alpha - lowRadian);//.RandWithFiveDigites();

            var zCoord = (Math.Sin(beta) * bottomL);//.RandWithFiveDigites();

            var length = (Math.Cos(beta) * bottomL);//.RandWithFiveDigites();

            var rototeAngle = MmToAngle(pos.Z);
            var rototeRadian = AngleToRadian(rototeAngle);

            var xCoord = (Math.Cos(rototeRadian) * length);//.RandWithFiveDigites();
            var yCoord = (Math.Sin(rototeRadian) * length);//.RandWithFiveDigites();

            return new Tuple<double, double, double>(xCoord, yCoord, zCoord);
        }

        public PosePosition ToPose(Tuple<double, double, double> coor)
        {
            var x = coor.Item1;
            var y = coor.Item2;
            var z = coor.Item3;

            var rotateZRadian = Math.Atan2(y, x);//.RandWithFiveDigites();
            var rotateZAngle = RadianToAngle(rotateZRadian);//.RandWithFiveDigites();
            var rotateZMM = (int)AngleToMM(rotateZAngle);

            var length = Math.Sqrt(x * x + y * y);//.RandWithFiveDigites();

            var bottomL = Math.Sqrt(length * length + z * z);//.RandWithFiveDigites();

            var alphaRadian = Math.Acos(bottomL / (2 * l));
            var highRadian = Math.PI - (2 * alphaRadian);

            var betaRadian = Math.Acos(length / bottomL);
            if (z < 0)
            {
                betaRadian = -1 * betaRadian;
            }

            var lowRadian = Math.PI - alphaRadian - betaRadian;
            var lowAngle = RadianToAngle(lowRadian);
            var lowMm = (int)AngleToMM(lowAngle - B1);

            var highAngle = (lowAngle - B1) + B2 - RadianToAngle(highRadian);
            var highMm = (int)AngleToMM(highAngle);

            return new PosePosition(lowMm, highMm, rotateZMM);
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
            return (angle * Math.PI / 180).RandWithFiveDigites();
        }

        public double RadianToAngle(double radian)
        {
            return (radian * 180 / Math.PI);//.RandWithFiveDigites();
        }

        public double TriangleCorner(double radian)
        {
            return ((Math.PI - radian) / 2);//.RandWithFiveDigites();
        }

        public double BottomOfIsoscelesTriangle(double l, double radian)
        {
            return (2 * l * Math.Cos(radian));//.RandWithFiveDigites();
        }

        public double HighAngle(int x, int y)
        {
            return B2 + MmToAngle(x) - MmToAngle(y);
        }

        public double LowAngle(int x)
        {
            return B1 + MmToAngle(x);
        }
    }
}
