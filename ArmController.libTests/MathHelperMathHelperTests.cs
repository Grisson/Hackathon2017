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
    public class MathHelperMathHelperTests
    {
        public static TouchPoint[][] GenerateTouchPoints()
        {
            var result = new List<TouchPoint[]>();

            result.Add(new[] { new TouchPoint(65, -365), new TouchPoint(88, -363.5), new TouchPoint(129, -360.5) });
            //result.Add(new Tuple<PosePosition, TouchPoint>(pose, new TouchPoint (218, -360)));

            result.Add(new[] { new TouchPoint(83.5, -334), new TouchPoint(104.5, -332.5), new TouchPoint(151.5, -330) });
            //result.Add(new Tuple<PosePosition, TouchPoint>(new PosePosition(8, 3175, 3650, 225), new TouchPoint(242.5, -332.5)));

            result.Add(new[] { new TouchPoint(99.5, -274.5), new TouchPoint(124, -272.5), new TouchPoint(172, -271) });
            //result.Add(new Tuple<PosePosition, TouchPoint>( new PosePosition(12, 3275, 3550, 175), new TouchPoint (267, -274.5)));

            result.Add(new[] { new TouchPoint(119, -160.5), new TouchPoint(143, -159.5), new TouchPoint(194.5, -159) });
            //result.Add(new Tuple<PosePosition, TouchPoint>( new PosePosition(14, 3475, 3350, 125), new TouchPoint (300.5, -163.5)));

            return result.ToArray();

        }

        public static TouchPoint[][] GenerateTouchPoints2()
        {
            var result = new List<TouchPoint[]>();

            result.Add(new[] { new TouchPoint(3, -10), new TouchPoint(4, -8.26), new TouchPoint(5, -8) });
            //result.Add(new Tuple<PosePosition, TouchPoint>(pose, new TouchPoint (218, -360)));

            result.Add(new[] { new TouchPoint(3, -7.76), new TouchPoint(4, -7.17), new TouchPoint(5, -7) });
            //result.Add(new Tuple<PosePosition, TouchPoint>(new PosePosition(8, 3175, 3650, 225), new TouchPoint(242.5, -332.5)));

            result.Add(new[] { new TouchPoint(3, -6.54), new TouchPoint(4, -6.12), new TouchPoint(5, -6) });
            //result.Add(new Tuple<PosePosition, TouchPoint>( new PosePosition(12, 3275, 3550, 175), new TouchPoint (267, -274.5)));

            result.Add(new[] { new TouchPoint(3, -5.42), new TouchPoint(4, -5.1), new TouchPoint(5, -5) });
            //result.Add(new Tuple<PosePosition, TouchPoint>( new PosePosition(14, 3475, 3350, 125), new TouchPoint (300.5, -163.5)));

            return result.ToArray();

        }

        //[TestMethod()]
        //public void CalculatorCentorOfCircleTest()
        //{
        //    //var points = GenerateTouchPoints();
        //    var points = GenerateTouchPoints2();
        //    var result = MathHelper.CalculateCentorOfCircle(points);
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.Length == 2);
        //}

        [TestMethod()]
        public void CalculatorCentorOfCircleTest2()
        {
            //var points = GenerateTouchPoints();
            var points = GenerateTouchPoints2();
            var result = MathHelper.CalculateCenterOfCircle(points);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 2);
            Assert.IsTrue((result[0] - 5) < 0.1);
            Assert.IsTrue((result[1] + 10) < 0.1);
        }
    }
}