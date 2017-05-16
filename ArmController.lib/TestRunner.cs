using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ArmController.lib.Data;
using Newtonsoft.Json;

namespace ArmController.lib
{
    public class TestRunner
    {
        private double _liftUpDistance = 5;
        private bool isTouchReported = false;

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

        public string CanResume()
        {
            if(isTouchReported)
            {
                var tmp = new ResumeCommand();
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                string serialized = JsonConvert.SerializeObject(tmp, settings);

                isTouchReported = !isTouchReported;

                return serialized;
            }
            else
            {
                return string.Empty;
            }
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
            isTouchReported = true;
            return true;
        }

        public void Calibrate()
        {
            PoseTouchMapping = Calibrator.MapPoseAndTouch(PosePositions, TouchPoints);

            // test agent location
            var lines = Calibrator.CalculatorBisectorLines(PoseTouchMapping);
            var points = Calibrator.CalculatorCenterPoints(lines);
            AgentLocation = MathHelper.MeanPoint(points);

            // arm position function
            // ??????
        }

        public string GetCalibrationCommonds()
        {
            var commonds = new List<BaseCommand>();


            // disable this
            commonds.Add(new GCommand(15, 12.3, 0));

            // Z
            // lift up
            commonds.Add(new GCommand(-5, 0, 0));
            // rotate 
            //commonds.Add(new GCommand(0, 0, 5));
            // Touch Down
            commonds.Add(new GCommand(5, 0, 0));

            commonds.Add(new PauseCommand(30, 5000));

            // lift up
            commonds.Add(new GCommand(-5, 0, 0));
            // rotate 
            //commonds.Add(new GCommand(0, 0, 5));
            // Touch Down
            commonds.Add(new GCommand(5, 0, 0));

            commonds.Add(new PauseCommand(30, 5000));


            commonds.Add(new GCommand(-5, 0, 0));
            // rotate 
            //commonds.Add(new GCommand(0, 0, 5));
            // Touch Down
            commonds.Add(new GCommand(5, 0, 0));

            commonds.Add(new PauseCommand(30, 5000));

            //// x, y
            //// lift up
            //commonds.Add(new GCommand(-5, 0, 0));
            //// Adjust length
            //commonds.Add(new GCommand(2, -2, 0));
            //// Touch Down
            //commonds.Add(new GCommand(5, 0, 0));

            //// z
            //commonds.Add(new GCommand(-5, 0, 0));
            //// rotate 
            //commonds.Add(new GCommand(0, 0, -2));
            //commonds.Add(new GCommand(5, 0, 0));

            //// lift up
            //commonds.Add(new GCommand(-5, 0, 0));
            //// rotate 
            //commonds.Add(new GCommand(0, 0, -3));
            //// Touch Down
            //commonds.Add(new GCommand(5, 0, 0));

            //// lift up
            //commonds.Add(new GCommand(-5, 0, 0));

            //commonds.Add(new GCommand(1, -1, 0));

            //// Touch Down
            //commonds.Add(new GCommand(5, -0, 0));

            //// lift up
            //commonds.Add(new GCommand(-5, 0, 0));

            //// Touch Down
            ////commonds.Add(new GCommand(-3, 0, 0));
            //commonds.Add(new GCommand(-13, -10, 0));


            if (commonds.Count <= 0)
            {
                return string.Empty;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string serialized = JsonConvert.SerializeObject(commonds, settings);

            return serialized;
            //List<Base> deserializedList = JsonConvert.DeserializeObject<List<Base>>(Serialized, settings);
        }

        public GCommand CreateLiftUpCommand(double dist)
        {
            return new GCommand(dist, 0, 0);
        }

        public GCommand CreateTouchDownCommand(double dist)
        {
            return new GCommand((-1) * dist, 0, 0);
        }
        
    }
}