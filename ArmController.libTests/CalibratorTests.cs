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

            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(1, 12.5, 14.8, 2.5), new TouchResponse (65, 365)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(2, 12.5, 14.8, 2.3), new TouchResponse (88, 363.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(3, 12.5, 14.8, 1.9), new TouchResponse (129, 360.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(4, 12.5, 14.8, 1.1), new TouchResponse (218, 360)));

            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(5, 12.7, 14.6, 2.3), new TouchResponse(83.5, 334)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(6, 12.7, 14.6, 2.1), new TouchResponse(104.5, 332.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(7, 12.7, 14.6, 1.7), new TouchResponse(151.5, 330)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(8, 12.7, 14.6, 0.9), new TouchResponse(242.5, 332.5)));

            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(9, 13.1, 14.2, 2.1), new TouchResponse(99.5, 274.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(10, 13.1, 14.2, 1.9), new TouchResponse(124, 272.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(11, 13.1, 14.2, 1.5), new TouchResponse(172, 271)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(12, 13.1, 14.2, 0.7), new TouchResponse (267, 274.5)));

            result.Add(new Tuple<PosePosition, TouchResponse>(new PosePosition(10, 13.9, 13.4, 1.9), new TouchResponse(119, 160.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(13, 13.9, 13.4, 1.7), new TouchResponse(143, 159.5)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(14, 13.9, 13.4, 1.3), new TouchResponse (194.5, 159)));
            result.Add(new Tuple<PosePosition, TouchResponse>( new PosePosition(14, 13.9, 13.4, 0.5), new TouchResponse (300.5, 163.5)));

            return result;

        }

        [TestMethod()]
        public void MapPoseAndTouchTest()
        {
            Assert.Fail();
        }
    }
}