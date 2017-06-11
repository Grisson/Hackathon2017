using ArmController.lib.Data;
using ArmController.Models.Command;
using ArmController.Models.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmController.lib
{
    public class TestRunner
    {
        public PosePosition initialProbPose = new PosePosition(2265,3303, 0);//(2800, 2800, 0);
        public int ProbInterval = 7;
        public bool isProbing = false;

        public const string TaskNameCalibration = "Calib";

        private bool isTouchReported = false;

        internal TestTarget Target { get; set; }
        internal TestAgent Agent { get; set; }

        internal int CurrentTestCaseIndex { get; set; }
        internal TestCase CurrenTestCase { get; set; }
        internal List<TestCase> TestCases { get; set; }

        internal SortedList<long, PosePosition> PosePositions { get; set; }
        internal SortedList<long, TouchPoint> TouchPoints { get; set; }
        internal List<Tuple<PosePosition, TouchPoint>> PoseTouchMapping { get; set; }
        internal TouchPoint AgentLocation { get; set; }

        // Length = F(distance)
        // length = a + b * distance
        internal Tuple<double, double> F_Dist_Length { get; set; }

        // a + bx + cy = 0;
        internal Tuple<double, double, double> F_x { get; set; }

        public TestRunner()
        {
            PosePositions = new SortedList<long, PosePosition>();
            TouchPoints = new SortedList<long, TouchPoint>();
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

            if(isProbing)
            {
                ArmPositionCalculator.SharedInstance.ProbbedPose = PosePositions.Last().Value;
                ArmPositionCalculator.SharedInstance.ProbbedPose.X += CommandHelper.GetTapDistance();
                ArmPositionCalculator.SharedInstance.ProbbedPose.Y += CommandHelper.GetTapDistance();
                isProbing = false;
            }
            else
            {
                TouchPoints[timeStamp] = new TouchPoint(x, y);
                isTouchReported = true;
            }

            return true;
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

        public bool WaitingProbResult()
        {
            return ArmPositionCalculator.SharedInstance.IsProbDetected;
        }

        public void Calibrate()
        {
            PoseTouchMapping = Calibrator.MapPoseAndTouch(PosePositions, TouchPoints);

            // test agent corrdinate
            var pointsOnSameRadius = Calibrator.TouchPointsOnSameRadius(PoseTouchMapping);
            var centerCoordinate = MathHelper.CalculateCenterOfCircle(pointsOnSameRadius);
            AgentLocation = new TouchPoint { X = centerCoordinate[0], Y = centerCoordinate[1] };

            // rotate calibration -- no need

            // length calibration
            var pirsOnXAxis = Calibrator.TouchPairsOnXAxis(PoseTouchMapping);
            // TODO: may swith the X and Y in this function
            // length = a + b * distance
            F_Dist_Length = Calibrator.MapLength(pirsOnXAxis, AgentLocation);

            // a + bx + cy = 0;
            var pointsOnXAxis = Calibrator.TouchPointsOnXAxis(PoseTouchMapping);
            var ab = Calibrator.FindFx(pointsOnXAxis);
            F_x = new Tuple<double, double, double>(ab.Item1, ab.Item2, -1);
        }

        public void Done(string data)
        {
            switch (data)
            {
                case TaskNameCalibration:
                    Calibrate();
                    break;
                default:
                    break;
            }
        }

        public string GetSecondCalibrationCommonds()
        {
            PosePositions.Clear();
            TouchPoints.Clear();

            var commonds = new List<BaseCommand>();

            var lengths = new[] { 70, 80, 90, 100, 110, 120, 130 };  // coordinate_x
            var rotates = new[] { 90, 100, 110, 120, 130, 140, 150 }; // step

            var coor_Z = (int)ArmPositionCalculator.SharedInstance.ToCoordinate(ArmPositionCalculator.SharedInstance.ProbbedPose).Item3;


            var tapDistance = CommandHelper.GetTapDistance();
            for (var i = 0; i < lengths.Length; i++)
            {
                var length = lengths[i];
                var rotate = rotates[i];
                var pose = ArmPositionCalculator.SharedInstance.ToPose(new Tuple<double, double, double>(length, 0, coor_Z));
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

        public string GetProbCommands(int retry = 0)
        {
            isProbing = true;
            if (retry == 0)
            {
                ArmPositionCalculator.SharedInstance.ProbbedPose = null;
            }

            if(retry > 10)
            {
                return string.Empty;
            }

            var commonds = new List<BaseCommand>();
            var x = initialProbPose.X + retry * ProbInterval;
            var y = initialProbPose.Y + retry * ProbInterval;
            commonds.Add(new PoseCommand(x, y, 0));
            commonds.Tap();
            commonds.Add(new ProbWaitingCommand(10, 500, retry));

            if (commonds.Count <= 0)
            {
                return string.Empty;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string serialized = JsonConvert.SerializeObject(commonds, settings);

            return serialized;
        }

        public string GetTestTouchCommand()
        {
            var commonds = new List<BaseCommand>();

            var tapDistance = CommandHelper.GetTapDistance();
            var posePosition = ConvertTouchPointToPosition(new TouchPoint(100, -100));
            posePosition.X -= tapDistance;
            posePosition.Y -= tapDistance;

            commonds.Add(new PoseCommand(posePosition.X, posePosition.Y, posePosition.Z));
            commonds.Tap();
            commonds.Reset();

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string serialized = JsonConvert.SerializeObject(commonds, settings);

            return serialized;

        }

        public PosePosition ConvertCoordinatToPosition(Tuple<double, double, double> coordinate)
        {
            return ArmPositionCalculator.SharedInstance.ToPose(coordinate);
        }

        public Tuple<double, double, double> ConvertPositionToCoordinat(PosePosition pose)
        {
            return ArmPositionCalculator.SharedInstance.ToCoordinate(pose);
        }

        public PosePosition ConvertTouchPointToPosition(TouchPoint touchPoint)
        {
            // 1. distance
            var dist = MathHelper.CalculateEuclideanDistance(new[] { AgentLocation.X, AgentLocation.Y }, new[] { touchPoint.X, touchPoint.Y });

            // 2. map dist to length (coordinate X)
            // F_Dist_Length: length = a + b * distance
            var coor_X = F_Dist_Length.Item1 + F_Dist_Length.Item2 * dist;

            // 3. coordinate Z
            var coor_Z = 0;
            if (ArmPositionCalculator.SharedInstance.IsProbDetected)
            {
                coor_Z = (int)ArmPositionCalculator.SharedInstance.ToCoordinate(ArmPositionCalculator.SharedInstance.ProbbedPose).Item3;
            }

            // 4. convert coordinate to pose. At this point, the pose should contain X, Y. Z is till 0
            var pose = ConvertCoordinatToPosition(new Tuple<double, double, double>(coor_X, 0, coor_Z));

            // 5. calculate the pose Z
            var rotateRadian = ArmPositionCalculator.SharedInstance.RotateRadian(AgentLocation, touchPoint);
            var rotateAngle = ArmPositionCalculator.SharedInstance.RadianToAngle(rotateRadian);
            var rotateStep = ArmPositionCalculator.SharedInstance.AngleToMM(rotateAngle);

            pose.Z = rotateStep;

            return pose;
        }

        public void StartTest()
        {
            TestCases = new List<TestCase>();
            CurrentTestCaseIndex = 0;
        }

        public string[] GetNextTestCase()
        {
            //if (CurrentTestCaseIndex <= TestCases.Count)
            //{
            //    CurrenTestCase = TestCases[CurrentTestCaseIndex++];
            //    return Agent.ToCommond(CurrenTestCase.Steps);
            //}
            return new string[] { };
        }
    }
}