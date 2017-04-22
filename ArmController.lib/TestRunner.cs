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

        public bool ReportAgentStatus()
        {
            return true;
        }


        public bool ReportTouchBegin(int x, int y)
        {
            return true;
        }

        public bool ReportTouchEnd()
        {
            return true;
        }
    }
}