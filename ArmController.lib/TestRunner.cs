using System.Collections.Generic;
using ArmController.lib.Data;

namespace ArmController.lib
{
    public class TestRunner
    {
        internal TestAgent Agent { get; set; }
        internal TestCase CurrenTestCase { get; set; }

        internal int CurrentTestCaseIndex { get; set; }
        internal TestTarget Target { get; set; }
        internal List<TestCase> TestCases { get; set; }


        internal SortedList<long, PosePosition> PosePositions { get; set; }
        internal SortedList<long, TouchResponse> TouchPoints { get; set; }


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
            return new string[] {};
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
            // Pose Position and TouchResponse here
        }
    }
}