using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArmController.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmController.lib.Data;
using ArmController.Models.Data;

namespace ArmController.lib.Tests
{
    [TestClass()]
    public class CalibratorTests
    {

        public static List<Tuple<PosePosition, TouchPoint>> GeneratePostAndTouch()
        {
            var result = new List<Tuple<PosePosition, TouchPoint>>();

            var pose = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double> (70.0, 0.0, 0.0));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose, new TouchPoint (65, -365)));
            pose.Z = 20;
            result.Add(new Tuple<PosePosition, TouchPoint>(pose, new TouchPoint (88, -363.5)));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose, new TouchPoint (129, -360.5)));
            //result.Add(new Tuple<PosePosition, TouchPoint>(pose, new TouchPoint (218, -360)));

            var pose2 = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double> (100.0, 0.0, 0.0));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose2, new TouchPoint(83.5, -334)));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose2, new TouchPoint(104.5, -332.5)));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose2, new TouchPoint(151.5, -330)));
            //result.Add(new Tuple<PosePosition, TouchPoint>(new PosePosition(8, 3175, 3650, 225), new TouchPoint(242.5, -332.5)));

            var pose3 = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double> (130.0, 0.0, 0.0));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose3, new TouchPoint(99.5, -274.5)));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose3, new TouchPoint(124, -272.5)));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose3, new TouchPoint(172, -271)));
            //result.Add(new Tuple<PosePosition, TouchPoint>( new PosePosition(12, 3275, 3550, 175), new TouchPoint (267, -274.5)));

            var pose4 = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double> (150.0, 0.0, 0.0));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose4, new TouchPoint(119, -160.5)));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose4, new TouchPoint(143, -159.5)));
            result.Add(new Tuple<PosePosition, TouchPoint>(pose4, new TouchPoint (194.5, -159)));
            //result.Add(new Tuple<PosePosition, TouchPoint>( new PosePosition(14, 3475, 3350, 125), new TouchPoint (300.5, -163.5)));

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