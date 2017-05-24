using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArmController.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmController.lib.Data;

namespace ArmController.lib.Tests
{
    [TestClass()]
    public class CalibratorTests
    {

        public static List<Tuple<PosePosition, TouchResponse>> GeneratePostAndTouch()
        {
            var result = new List<Tuple<PosePosition, TouchResponse>>();

            var pose = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double> (70.0, 0.0, 0.0));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose, new TouchResponse (65, -365)));
            pose.Z = 20;
            result.Add(new Tuple<PosePosition, TouchResponse>(pose, new TouchResponse (88, -363.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose, new TouchResponse (129, -360.5)));
            //result.Add(new Tuple<PosePosition, TouchResponse>(pose, new TouchResponse (218, -360)));

            var pose2 = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double> (100.0, 0.0, 0.0));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose2, new TouchResponse(83.5, -334)));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose2, new TouchResponse(104.5, -332.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose2, new TouchResponse(151.5, -330)));
            //result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(8, 3175, 3650, 225), new TouchResponse(242.5, -332.5)));

            var pose3 = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double> (130.0, 0.0, 0.0));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose3, new TouchResponse(99.5, -274.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose3, new TouchResponse(124, -272.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose3, new TouchResponse(172, -271)));
            //result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(12, 3275, 3550, 175), new TouchResponse (267, -274.5)));

            var pose4 = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double> (150.0, 0.0, 0.0));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose4, new TouchResponse(119, -160.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose4, new TouchResponse(143, -159.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(pose4, new TouchResponse (194.5, -159)));
            //result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(14, 3475, 3350, 125), new TouchResponse (300.5, -163.5)));

            return result;

        }

        [TestMethod()]
        public void MapPoseAndTouchTest()
        {
            var angle = Math.Atan2(1, 1);
            Assert.IsTrue(angle == (Math.PI / 4));
            var angle2 = Math.Asin(0.5);

            var ThirdDegree = Math.PI / 6;
            Assert.IsTrue(angle2 == ThirdDegree);


            Assert.Fail();
        }

        [TestMethod()]
        public void CalculatorZTest()
        {
            var pts = GeneratePostAndTouch();
            var lines = Calibrator.MapPointsOnSameLine(pts);
            Assert.IsTrue(lines.Count == 5);
            var result = Calibrator.CalculatorZ(lines);
            Assert.IsTrue(1==1);
        }
    }
}