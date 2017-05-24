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
    public class MathHelperMathHelperTests
    {
        public static TouchResponse[][] GenerateTouchPoints()
        {
            var result = new List<TouchResponse[]>();

            result.Add(new[] { new TouchResponse(65, -365), new TouchResponse(88, -363.5), new TouchResponse(129, -360.5) });
            //result.Add(new Tuple<PosePosition, TouchResponse>(pose, new TouchResponse (218, -360)));

            result.Add(new[] { new TouchResponse(83.5, -334), new TouchResponse(104.5, -332.5), new TouchResponse(151.5, -330) });
            //result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(8, 3175, 3650, 225), new TouchResponse(242.5, -332.5)));

            result.Add(new[] { new TouchResponse(99.5, -274.5), new TouchResponse(124, -272.5), new TouchResponse(172, -271) });
            //result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(12, 3275, 3550, 175), new TouchResponse (267, -274.5)));

            result.Add(new[] { new TouchResponse(119, -160.5), new TouchResponse(143, -159.5), new TouchResponse(194.5, -159) });
            //result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(14, 3475, 3350, 125), new TouchResponse (300.5, -163.5)));

            return result.ToArray();

        }

        public static TouchResponse[][] GenerateTouchPoints2()
        {
            var result = new List<TouchResponse[]>();

            result.Add(new[] { new TouchResponse(3, -10), new TouchResponse(4, -8.26), new TouchResponse(5, -8) });
            //result.Add(new Tuple<PosePosition, TouchResponse>(pose, new TouchResponse (218, -360)));

            result.Add(new[] { new TouchResponse(3, -7.76), new TouchResponse(4, -7.17), new TouchResponse(5, -7) });
            //result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(8, 3175, 3650, 225), new TouchResponse(242.5, -332.5)));

            result.Add(new[] { new TouchResponse(3, -6.54), new TouchResponse(4, -6.12), new TouchResponse(5, -6) });
            //result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(12, 3275, 3550, 175), new TouchResponse (267, -274.5)));

            result.Add(new[] { new TouchResponse(3, -5.42), new TouchResponse(4, -5.1), new TouchResponse(5, -5) });
            //result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(14, 3475, 3350, 125), new TouchResponse (300.5, -163.5)));

            return result.ToArray();

        }

        [TestMethod()]
        public void CalculatorCentorOfCircleTest()
        {
            //var points = GenerateTouchPoints();
            var points = GenerateTouchPoints2();
            var result = MathHelper.CalculatorCentorOfCircle(points);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 2);
        }

        [TestMethod()]
        public void CalculatorCentorOfCircleTest2()
        {
            //var points = GenerateTouchPoints();
            var points = GenerateTouchPoints2();
            var result = MathHelper.CalculatorCentorOfCircle(points);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == 2);
            Assert.IsTrue((result[0] - 5) < 0.1);
            Assert.IsTrue((result[1] + 10) < 0.1);
        }
    }
}