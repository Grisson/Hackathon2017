using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ArmController.lib.Data;

namespace ArmController.lib
{
    public class TestRunner
    {
        private const double Tolerance = 0.00001;

        internal TestTarget Target { get; set; }
        internal TestAgent Agent { get; set; }

        internal int CurrentTestCaseIndex { get; set; }
        internal TestCase CurrenTestCase { get; set; }
        internal List<TestCase> TestCases { get; set; }

        internal SortedList<long, PosePosition> PosePositions { get; set; }
        internal SortedList<long, TouchResponse> TouchPoints { get; set; }
        internal List<Tuple<PosePosition, TouchResponse>> PoseTouchMapping { get; set; }
        internal Point AgentLocation { get; set; } 

        public TestRunner()
        {
            PosePositions = new SortedList<long, PosePosition>();
            TouchPoints = new SortedList<long, TouchResponse>();
        }

        public void RegisterTestAgent(string agentId)
        {
            Agent = string.IsNullOrEmpty(agentId) ? new TestAgent() : new TestAgent(agentId);
        }

        public void UnRegisterTestAgent()
        {
            Agent = null;
        }

        public void RegisterTestTarget(string os = "ios")
        {
            Target = new TestTarget
            {
                DeviceOperationSystem = os
            };
        }

        public void StartTest()
        {
            TestCases = new List<TestCase>();
            CurrentTestCaseIndex = 0;
        }

        public string[] GetNextTestCase()
        {
            if (CurrentTestCaseIndex <= TestCases.Count)
            {
                CurrenTestCase = TestCases[CurrentTestCaseIndex++];
                return Agent.ToCommond(CurrenTestCase.Steps);
            }
            return new string[] { };
        }

        public bool ReportAgentPosePosition(long timeStamp, double x, double y, double z)
        {
            if (Agent == null)
            {
                return false;
            }

            Agent.CurrentPosition = new PosePosition(timeStamp, x, y, z);
            PosePositions[timeStamp] = Agent.CurrentPosition;

            return true;
        }

        public bool ReportTouchBegin(long timeStamp, double x, double y)
        {
            TouchPoints[timeStamp] = new TouchResponse(x, y);
            return true;
        }

        public bool ReportTouchEnd()
        {
            return true;
        }

        public void Calibrate()
        {
            MapPoseAndTouch();

            // find test agent location
            var lines = CalculatorBisectorLines();
            var points = CalculatorCenterPoints(lines);
            AgentLocation = MathHelper.MeanPoint(points);

            // regression arm
        }

        public void MapPoseAndTouch()
        {
            PoseTouchMapping = new List<Tuple<PosePosition, TouchResponse>>();

            // Pose Position and TouchResponse here
            var poseTimestamp = PosePositions.Keys;
            var touchTimestamp = TouchPoints.Keys;

            var poseIndex = 0;
            var touchIndex = 0;

            for (; poseIndex < poseTimestamp.Count && touchIndex < touchTimestamp.Count;)
            {
                if (touchTimestamp[touchIndex] < poseTimestamp[poseIndex])
                {
                    touchIndex++;
                }
                else if ((poseTimestamp[poseIndex] < touchTimestamp[touchIndex]) &&
                    ((poseIndex + 1 >= poseTimestamp.Count) || (touchTimestamp[touchIndex] < poseTimestamp[poseIndex + 1])))
                {

                    PoseTouchMapping.Add(new Tuple<PosePosition, TouchResponse>(PosePositions[poseTimestamp[poseIndex]], TouchPoints[touchTimestamp[touchIndex]]));

                    poseIndex++;
                    touchIndex++;
                }
                else
                {
                    poseIndex++;
                }
            }
        }


        public List<Tuple<double, double, double>> CalculatorBisectorLines()
        {
            var lines = new List<Tuple<double, double, double>>();

            for (var i = 0; i < PoseTouchMapping.Count - 1; i++)
            {
                for (var j = i + 1; j < PoseTouchMapping.Count; j++)
                {
                    var agentAPosition = PoseTouchMapping[i].Item1;
                    var agentBPosition = PoseTouchMapping[j].Item1;

                    if ((Math.Abs(agentAPosition.X - agentBPosition.X) < Tolerance) && 
                        (Math.Abs(agentAPosition.Y - agentBPosition.Y) < Tolerance) &&
                        (Math.Abs(agentAPosition.Z - agentBPosition.Z) > Tolerance))
                    {
                        // touch point should on same cycle

                        var touchPointA = PoseTouchMapping[i].Item2;
                        var touchPointB = PoseTouchMapping[j].Item2;

                        var line = MathHelper.CalculatorPerpendicularBisector(touchPointA, touchPointB);
                        lines.Add((line));
                    }
                }
            }

            return lines;
        }

        public List<Point> CalculatorCenterPoints(List<Tuple<double, double, double>> lines)
        {
            var points = new List<Point>();

            for (var i = 0; i < lines.Count - 1; i++)
            {
                for (var j = i+1; j < lines.Count; j++)
                {
                    var p = MathHelper.Intersect(lines[i], lines[j]);
                    points.Add(p);
                }
            }

            return points;
        }
    }
}