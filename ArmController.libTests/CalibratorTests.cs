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

            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(1, 3125, 3700, 625), new TouchResponse (65, -365)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(2, 3125, 3700, 575), new TouchResponse (88, -363.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(3, 3125, 3700, 475), new TouchResponse (129, -360.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(4, 3125, 3700, 275), new TouchResponse (218, -360)));

            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(5, 3175, 3650, 575), new TouchResponse(83.5, -334)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(6, 3175, 3650, 525), new TouchResponse(104.5, -332.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(7, 3175, 3650, 425), new TouchResponse(151.5, -330)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(8, 3175, 3650, 225), new TouchResponse(242.5, -332.5)));

            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(9, 3275, 3550, 525), new TouchResponse(99.5, -274.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(10, 3275, 3550, 475), new TouchResponse(124, -272.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(11, 3275, 3550, 375), new TouchResponse(172, -271)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(12, 3275, 3550, 175), new TouchResponse (267, -274.5)));

            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(10, 3475, 3350, 475), new TouchResponse(119, -160.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(13, 3475, 3350, 425), new TouchResponse(143, -159.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(14, 3475, 3350, 325), new TouchResponse (194.5, -159)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(14, 3475, 3350, 125), new TouchResponse (300.5, -163.5)));

            return result;

        }

        [TestMethod()]
        public void MapPoseAndTouchTest()
        {
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