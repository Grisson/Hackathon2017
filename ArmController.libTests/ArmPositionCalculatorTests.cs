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
        [TestMethod()]
        public void AngleToMMTest()
        {
            var steps = ArmPositionCalculator.SharedInstacne.AngleToMM(90);

            Assert.IsTrue(steps == 3600);
        }
    }
}