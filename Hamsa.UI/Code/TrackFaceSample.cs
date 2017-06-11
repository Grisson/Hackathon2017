using Hamsa.Azure;
using Hamsa.Common;
using Hamsa.Device;
using System;
using System.Linq;
using System.Threading;

namespace Hamsa.UI.Code
{
    public class TrackFaceSample : BaseExecutableCode
    {
        public Camera Eye;
        public ThreeDOFArm Arm;
        public Cognitive Brain;

        public override void Setup()
        {
            Eye = new Camera(0);
            Eye.Start();

            Arm = new ThreeDOFArm("COM4", 115200);
            Arm.Connect();

            Brain = new Cognitive();
        }

        public override void Loop()
        {
            var img = Eye.GetLatestData();

            var faces = Brain.DetectFaces(img);

            if(faces.Length > 0)
            {
                var firstFace = faces.First();

                // map the location
                var x = firstFace.FaceRectangle.Left + firstFace.FaceRectangle.Width / 2;
                var y = firstFace.FaceRectangle.Top + firstFace.FaceRectangle.Height / 2;
                var coordinateX = 80;
                var coordinateZ = 60 + 120 * (1 - (y * 1.0) / img.Height);
                var pose = Arm.ToPose(new Tuple<double, double, double>(coordinateX, 0, coordinateZ));

                var rotate = 45 + 90 * (1 - x * 1.0 / img.Width);
                var rotateStep = Arm.AngleToMM(rotate);
                pose.MotorThreeSteps = rotateStep;

                // Command ARM
                Arm.MoveTo(pose);
            }
            else
            {
                Console.WriteLine("Face is not detected!");
            }

            //Thread.Sleep(10);
        }

        public override void Cleanup()
        {
            Eye.Dispose();
            Arm.Dispose();
            Brain = null;
        }
    }
}
