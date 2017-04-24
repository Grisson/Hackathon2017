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
    public class MathHelperMathHelperTests
    {
        [TestMethod()]
        public void CalculatorPerpendicularBisectorPerpendicularBisectorTest()
        {
            var pointA = new ArmController.lib.Data.TouchResponse(6, 5);
            var pointB = new ArmController.lib.Data.TouchResponse(0, 1);
            var line = MathHelper.CalculatorPerpendicularBisector(pointA, pointB);

            Assert.IsTrue(Math.Abs(line.Item1 % 3) < 0.0000000000001);
            Assert.IsTrue(Math.Abs(line.Item2 % 2) < 0.0000000000001);
            Assert.IsTrue(Math.Abs(line.Item1 % 15) < 0.0000000000001);
        }
    }
}