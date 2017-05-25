using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ArmController.lib.Data;
using Newtonsoft.Json;

namespace ArmController.lib
{
    public class TestRunner
    {
        public const string TaskNameCalibration = "Calib";
        public const string TaskNameCalibrationZ = "CalibZ";

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
        internal Tuple<double, double> F_length { get; set; }
        internal Tuple<double, double> F_x { get; set; }

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
            if (isTouchReported)
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

        public bool ReportAgentPosePosition(long timeStamp, int x, int y, int z)
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

            // test agent corrdinate
            var pointsOnSameRadius = Calibrator.TouchPointsOnSameRadius(PoseTouchMapping);
            var centerCoordinate = MathHelper.CalculateCenterOfCircle(pointsOnSameRadius);
            AgentLocation = new Point { X = centerCoordinate[0], Y = centerCoordinate[1] };

            // rotate calibration -- no need

            // length calibration
            var pirsOnXAxis = Calibrator.TouchPairsOnXAxis(PoseTouchMapping);
            // TODO: may swith the X and Y in this function
            F_length = Calibrator.MapLength(pirsOnXAxis, AgentLocation);

            // fx 
            var pointsOnXAxis = Calibrator.TouchPointsOnXAxis(PoseTouchMapping);
            F_x = Calibrator.FindFx(pointsOnXAxis);
        }



        public void Done(string data)
        {
            switch (data)
            {
                case TaskNameCalibration:
                    Calibrate();
                    break;
                case TaskNameCalibrationZ:
                    //CalibrateZ();
                    break;
                default:
                    break;
            }
        }

        public string GetCalibrationCommonds()
        {
            var commonds = new List<BaseCommand>();

            // disable this
            //commonds.Add(new GCommand(12.9, 14.7, 0));
            var pose = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double>(60, 0, 0));

            commonds.Add(new GCommand(3171, 2371, 0));

            commonds.TouchInStairs(new List<int> { 0, -100, -100, -100, -100 }, new List<int> { 0, 100, 100, 100, 100 });

            commonds.TouchInStairs(new List<int> { -100, -100, -100, -100 }, new List<int> { -100, -100, -100, -100 });

            commonds.Reset();

            commonds.Add(new DoneCommand(TaskNameCalibrationZ));

            commonds.Add(new PauseCommand(30, -1));

            commonds.Add(new GCommand()
            {
                ResetPosition = true
            });

            commonds.Add(new DoneCommand(TaskNameCalibration));

            if (commonds.Count <= 0)
            {
                return string.Empty;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string serialized = JsonConvert.SerializeObject(commonds, settings);

            return serialized;
            //List<Base> deserializedList = JsonConvert.DeserializeObject<List<Base>>(Serialized, settings);
        }

        public string GetSecondCalibrationCommonds()
        {
            var commonds = new List<BaseCommand>();

            var lengths = new[] { 60, 70, 80, 90, 100, 110, 120 };
            var rotates = new[] { 10, 15, 20, 25, 30, 35, 40 };

            var tapDistance = CommandHelper.GetTapDistance();
            for (var i = 0; i < lengths.Length; i++)
            {
                var length = lengths[i];
                var rotate = rotates[i];
                var pose = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double>(length, 0, 0));
                var x = pose.X - tapDistance;
                var y = pose.Y - tapDistance;

                commonds.Add(new PoseCommand(x, y, 0));
                commonds.Tap();
                commonds.WaitForTouch();

                commonds.TouchInSameRadius(new List<int> { rotate, -2 * rotate });

                commonds.Rotate(rotate);
            }

            commonds.Reset();

            commonds.Add(new DoneCommand(TaskNameCalibration));

            commonds.Add(new PauseCommand(30, -1));

            if (commonds.Count <= 0)
            {
                return string.Empty;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string serialized = JsonConvert.SerializeObject(commonds, settings);

            return serialized;
            //List<Base> deserializedList = JsonConvert.DeserializeObject<List<Base>>(Serialized, settings);
        }

        public PosePosition ConvertCoordinatToPosition(Tuple<double, double, double> coordinate)
        {
            return ArmPositionCalculator.SharedInstance.ToPose(coordinate);
        }

        public Tuple<double, double, double> ConvertPositionToCoordinat(PosePosition pose)
        {
            return ArmPositionCalculator.SharedInstance.ToCoordinate(pose);
        }

        public PosePosition ConvertTouchPointToPosition(TouchResponse touchPoint)
        {
            // 1. distance
            var dist = MathHelper.CalculateDistance(new[] { AgentLocation.X, AgentLocation.Y }, new[] { touchPoint.X, touchPoint.Y });

            // 2. map to coordinate X

            // 3. calculate the Z

            // 4. coordinate to distance

            return null;
        }

        //
        // there is no need to calibrate Z 
        //
        [Obsolete("no need")]
        public void CalibrateZ()
        {
            //PoseTouchMapping = Calibrator.MapPoseAndTouch(PosePositions, TouchPoints);

            //// rotate calibration
            //var pointsOnSameLine = Calibrator.MapPointsOnSameLine(PoseTouchMapping);
            //Fz = Calibrator.CalculatorZ(pointsOnSameLine);
        }
    }
}