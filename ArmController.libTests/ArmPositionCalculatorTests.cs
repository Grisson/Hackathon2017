using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArmController.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib.Tests
{
    [TestClass()]
    public class ArmPositionCalculatorTests
    {
        public static double Tolerance = 0.01;

        [TestMethod()]
        public void AngleToMMTest()
        {
            var steps = ArmPositionCalculator.SharedInstacne.AngleToMM(90);

            Assert.IsTrue(steps == 3600);
        }

        [TestMethod()]
        public void TestPositionToCoord()
        {
            var coord = ArmPositionCalculator.SharedInstacne.ToCoordinate(new Data.PosePosition(680, 1040, 0));

            Assert.IsNotNull(coord);
            Assert.IsTrue(Math.Abs(coord.Item1 - 60) < Tolerance);
            Assert.IsTrue(coord.Item2 == 0);
            Assert.IsTrue(Math.Abs(coord.Item3 - 120 * Math.Sqrt(3) / 2) < Tolerance);
        }

        [TestMethod()]
        public void TestPositionToCoord2()
        {
            var coord = ArmPositionCalculator.SharedInstacne.ToCoordinate(new Data.PosePosition(680, 1040, 2400));

            Assert.IsNotNull(coord);
            Assert.IsTrue(Math.Abs(coord.Item1 - 60/2) < Tolerance);
            Assert.IsTrue(Math.Abs(coord.Item2 - 60 * Math.Sqrt(3)/2) < Tolerance);
            Assert.IsTrue(Math.Abs(coord.Item3 - 120 * Math.Sqrt(3)/2) < Tolerance);
        }

        [TestMethod()]
        public void TestCoordToPose()
        {
            var initPose = new Data.PosePosition(680, 1040, 2400);
            var coord = ArmPositionCalculator.SharedInstacne.ToCoordinate(new Data.PosePosition(680, 1040, 2400));
            var postPose = ArmPositionCalculator.SharedInstacne.ToPose(coord);

            Assert.IsNotNull(postPose);
            Assert.IsTrue(Math.Abs(postPose.X - initPose.X) < Tolerance);
            Assert.IsTrue(Math.Abs(postPose.Y - initPose.Y) < Tolerance);
            Assert.IsTrue(Math.Abs(postPose.Z - initPose.Z) < Tolerance);
        }
    }
}