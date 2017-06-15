    using Hamsa.Common;
using Hamsa.Common.Data;
using System;
using System.IO.Ports;


namespace Hamsa.Device
{
    public class ThreeDOFArm : UsbDevice, IPull<PosePosition>
    {
        public double GearRatio = 4.5;
        public int MotorStepsPerRev = 200;
        public int MicroStepSetting = 16;
        public double StepsPerMM => 1;

        public double StepsPerRev => GearRatio * MotorStepsPerRev * MicroStepSetting;

        public double AngelPerStep => 360.0 / StepsPerRev;

        public double B1 = 33; //43; // lower arm init degree
        public double B2 = 60; //69; //  high arm init degree

        public int l = 120; // 120mm arm


        public PosePosition CurrentPose { get; protected set; }
        public PosePosition TargetPose { get; set; }

        public ThreeDOFArm(string portName, int baudRate) : base(portName, baudRate)
        {
            CurrentPose = PosePosition.InitializePosition();
        }

        protected override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var port = sender as SerialPort;
                while (port.BytesToRead > 0)
                {
                    var d = port.ReadLine();

                    if (d.Equals("OK\r", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (TargetPose != null)
                        {
                            CurrentPose = TargetPose;
                            TargetPose = null;
                        }
                    }

                    DataReceivedHandler?.Invoke(d);
                }
            }
            finally
            {
                lock(Syncroot)
                {
                    IsIdel = true;
                }
            }
        }

        public void MoveTo(ThreeDimensionCoordinates point)
        {
            // coordinate to pose
            var pose = ConvertToPose(new Tuple<double, double, double>(point.X, point.Y, point.Z));
            // Go to pose
            MoveTo(pose);
        }

        public void MoveTo(PosePosition pose)
        {
            TargetPose = pose;
            var command = ConvertToGCommand(pose);
            Push(command);
        }

        public void ResetPosePosition()
        {
            TargetPose = PosePosition.InitializePosition();
            Push("G90 X0 Y0 Z0");
        }

        public string ConvertToGCommand(PosePosition TargetPose)
        {
            return $"G90 X{TargetPose.X} Y{TargetPose.Y} Z{TargetPose.Z}";
        }

        public Tuple<double, double, double> ConvertToCoordinate(PosePosition pos)
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

        public PosePosition ConvertToPose(Tuple<double, double, double> coor)
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

             var position = new PosePosition(lowMm, highMm, rotateZMM);

            return position;
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
            return Math.Round((angle * Math.PI / 180), 5, MidpointRounding.AwayFromZero);
        }

        public double RadianToAngle(double radian)
        {
            return (radian * 180 / Math.PI);//.RandWithFiveDigites();
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

        public PosePosition GetLatestData()
        {
            return CurrentPose.Clone();
        }

        public override void CleanUp()
        {
            if(Device != null && Device.IsOpen)
            {
                ResetPosePosition();
            }

            base.CleanUp();
        }

        //public double TriangleCorner(double radian)
        //{
        //    return ((Math.PI - radian) / 2);//.RandWithFiveDigites();
        //}

        //public double RotateRadian(TouchPoint center, TouchPoint pont)
        //{
        //    var deltaY = Math.Abs(center.Y - pont.Y);
        //    var deltaX = center.X - pont.X;

        //    return Math.Atan2(deltaX, deltaY);
        //}
    }

    public class PosePosition
    {
        public int MotorOneSteps { get; set; }
        public int MotorTwoSteps { get; set; }
        public int MotorThreeSteps { get; set; }
        public long TimeStamp;

        public int X => MotorOneSteps;
        public int Y => MotorTwoSteps;
        public int Z => MotorThreeSteps;

        public PosePosition() : this(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, 0, 0)
        {

        }

        public PosePosition(int x, int y, int z)
            : this(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), x, y, z)
        {

        }

        public PosePosition(long timeStamp, int x, int y, int z)
        {
            TimeStamp = timeStamp;
            MotorOneSteps = x;
            MotorTwoSteps = y;
            MotorThreeSteps = z;
        }

        public PosePosition Clone()
        {
            return new PosePosition()
            {
                MotorOneSteps = this.X,
                MotorTwoSteps = this.Y,
                MotorThreeSteps = this.Z,
            };
        }

        public PosePosition Incremental(int x, int y, int z)
        {
            return new PosePosition()
            {
                MotorOneSteps = this.X + x,
                MotorTwoSteps = this.Y + y,
                MotorThreeSteps = this.Z + z,
            };
        }

        public static PosePosition InitializePosition()
        {
            return new PosePosition(0, 0, 0);
        }
    }
}
