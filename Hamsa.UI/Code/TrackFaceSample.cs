using Hamsa.Azure;
using Hamsa.Common;
using Hamsa.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            Brain = new Cognitive();
        }

        public override void Loop()
        {
            var img = Eye.GetLatestData();

            var faces = Brain.DetectFaces(img);

            if(faces.Length > 0)
            {
                var firstFace = faces.First();
                Console.WriteLine($"{firstFace.FaceRectangle.Left}, {firstFace.FaceRectangle.Top}, {firstFace.FaceRectangle.Width}, {firstFace.FaceRectangle.Top}");
            }
            else
            {
                Console.WriteLine("Face is not detected!");
            }

            // map the location

            // Command ARM

            Thread.Sleep(1000);
        }

        public override void Cleanup()
        {
            Eye.Dispose();
            Arm.Dispose();
            Brain = null;
        }
    }
}
