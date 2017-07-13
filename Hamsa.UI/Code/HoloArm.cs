using ArmController.Models;
using ArmController.Models.Command;
using Hamsa.Common;
using Hamsa.Device;
using Hamsa.REST;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Hamsa.UI.Code
{
    public class HoloArm : BaseExecutableCode
    {
        //[Flags]
        public enum Status
        {
            Idle,
            Executing,
        }

        //public Camera Eye;
        public ThreeDOFArm LeftArm;
        public ThreeDOFArm RightArm;

        public long LeftArmId;
        public long RightArmId;

        public CloudBrain Brain;

        public Queue<BaseCommand> LeftArmCommandList;
        public BaseCommand LeftArmCurrentCommand;
        public Status LeftArmCurrentStatus;

        public Queue<BaseCommand> RightArmCommandList;
        public BaseCommand RightArmCurrentCommand;
        public Status RightArmCurrentStatus;

        public override void Setup()
        {
            LeftArmCommandList = new Queue<BaseCommand>();
            LeftArmCurrentStatus = Status.Idle;

            RightArmCommandList = new Queue<BaseCommand>();
            RightArmCurrentStatus = Status.Idle;

            Brain = new CloudBrain(new Uri("http://10.125.169.141:8182"), new BasicAuthenticationCredentials());

            RightArmId = 4396;
            LeftArmId = 43967;// Brain.Arm.Register().Value;

            LeftArm = new ThreeDOFArm("COM4", 115200);
            LeftArm.Subscript("CallBack", HandleLeftArmCallback);
            LeftArm.Connect();

            RightArm = new ThreeDOFArm("COM4", 115200);
            RightArm.Subscript("CallBack", HandleRightArmCallback);
            RightArm.Connect();
        }

        public override void Loop()
        {
            switch (LeftArmCurrentStatus)
            {
                case Status.Idle:
                    if (LeftArmCommandList.Count > 0)
                    {
                        ExecuteNextLeftArmCommand();
                    }
                    else
                    {
                        GetLeftArmNewCommands();
                    }
                    break;
                case Status.Executing:
                    break;
                default:
                    Thread.Yield();
                    break;
            }

            switch (RightArmCurrentStatus)
            {
                case Status.Idle:
                    if (RightArmCommandList.Count > 0)
                    {
                        ExecuteNextRightArmCommand();
                    }
                    else
                    {
                        GetRightArmNewCommands();
                    }
                    break;
                case Status.Executing:
                    break;
                default:
                    Thread.Yield();
                    break;
            }
        }

        public void GetLeftArmNewCommands()
        {
            // get new command
            var newCommandString = Brain.Arm.GetNextTask(LeftArmId);

            if (string.IsNullOrEmpty(newCommandString))
            {
                return;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var newCommands = JsonConvert.DeserializeObject<List<BaseCommand>>(newCommandString, settings);

            foreach (var c in newCommands)
            {
                LeftArmCommandList.Enqueue(c);
            }
        }

        public void GetRightArmNewCommands()
        {
            // get new command
            var newCommandString = Brain.Arm.GetNextTask(RightArmId);

            if (string.IsNullOrEmpty(newCommandString))
            {
                return;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var newCommands = JsonConvert.DeserializeObject<List<BaseCommand>>(newCommandString, settings);

            foreach (var c in newCommands)
            {
                RightArmCommandList.Enqueue(c);
            }
        }

        public void ExecuteNextRightArmCommand()
        {
            lock (SyncRoot)
            {
                RightArmCurrentCommand = RightArmCommandList.Dequeue();
                RightArmCurrentStatus = Status.Executing;
            }

            switch (RightArmCurrentCommand.Type)
            {
                case CommandType.Pose:
                    var command = RightArmCurrentCommand as PoseCommand;
                    command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    RightArm.MoveTo(new PosePosition() { MotorOneSteps = command.NextPosePosition.X, MotorTwoSteps = command.NextPosePosition.Y, MotorThreeSteps = command.NextPosePosition.Z });
                    break;
                case CommandType.GCode:
                    var gcommand = RightArmCurrentCommand as GCommand;
                    gcommand.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var newPose = new PosePosition()
                    {
                        MotorOneSteps = RightArm.CurrentPose.X + gcommand.XDelta,
                        MotorTwoSteps = RightArm.CurrentPose.Y + gcommand.YDelta,
                        MotorThreeSteps = RightArm.CurrentPose.Z + gcommand.ZDelta,
                    };
                    RightArm.MoveTo(newPose);
                    break;

                default:
                    lock (SyncRoot)
                    {
                        RightArmCurrentCommand = null;
                        RightArmCurrentStatus = Status.Idle;
                    }
                    break;
            }
        }

        public void ExecuteNextLeftArmCommand()
        {
            lock (SyncRoot)
            {
                LeftArmCurrentCommand = LeftArmCommandList.Dequeue();
                LeftArmCurrentStatus = Status.Executing;
            }

            switch (LeftArmCurrentCommand.Type)
            {
                case CommandType.Pose:
                    var command = LeftArmCurrentCommand as PoseCommand;
                    command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    LeftArm.MoveTo(new PosePosition() { MotorOneSteps = command.NextPosePosition.X, MotorTwoSteps = command.NextPosePosition.Y, MotorThreeSteps = command.NextPosePosition.Z });
                    break;
                case CommandType.GCode:
                    var gcommand = LeftArmCurrentCommand as GCommand;
                    gcommand.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var newPose = new PosePosition()
                    {
                        MotorOneSteps = LeftArm.CurrentPose.X + gcommand.XDelta,
                        MotorTwoSteps = LeftArm.CurrentPose.Y + gcommand.YDelta,
                        MotorThreeSteps = LeftArm.CurrentPose.Z + gcommand.ZDelta,
                    };
                    LeftArm.MoveTo(newPose);
                    break;
                
                default:
                    lock (SyncRoot)
                    {
                        LeftArmCurrentCommand = null;
                        LeftArmCurrentStatus = Status.Idle;
                    }
                    break;
            }
        }

        private void HandleRightArmCallback(string obj)
        {
            if (RightArmCurrentStatus == Status.Executing)
            {
                var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (RightArmCurrentCommand is GCommand)
                {
                    var gcommand = RightArmCurrentCommand as GCommand;
                    timeStamp = gcommand.SendTimeStamp;
                }
                else if (RightArmCurrentCommand is PoseCommand)
                {
                    var gcommand = RightArmCurrentCommand as PoseCommand;
                    timeStamp = gcommand.SendTimeStamp;
                }
                Brain.Arm.ReportPose(RightArmId,
                    timeStamp.ToString(),
                    RightArm.CurrentPose.MotorOneSteps,
                    RightArm.CurrentPose.MotorTwoSteps,
                    RightArm.CurrentPose.MotorThreeSteps);

                lock (SyncRoot)
                {
                    RightArmCurrentCommand = null;
                    RightArmCurrentStatus = Status.Idle;
                }
            }
        }

        private void HandleLeftArmCallback(string obj)
        {
            if (LeftArmCurrentStatus == Status.Executing)
            {
                var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (LeftArmCurrentCommand is GCommand)
                {
                    var gcommand = LeftArmCurrentCommand as GCommand;
                    timeStamp = gcommand.SendTimeStamp;
                }
                else if (LeftArmCurrentCommand is PoseCommand)
                {
                    var gcommand = LeftArmCurrentCommand as PoseCommand;
                    timeStamp = gcommand.SendTimeStamp;
                }
                Brain.Arm.ReportPose(LeftArmId,
                    timeStamp.ToString(),
                    LeftArm.CurrentPose.MotorOneSteps,
                    LeftArm.CurrentPose.MotorTwoSteps,
                    LeftArm.CurrentPose.MotorThreeSteps);

                lock (SyncRoot)
                {
                    LeftArmCurrentCommand = null;
                    LeftArmCurrentStatus = Status.Idle;
                }
            }
        }

        public override void Cleanup()
        {
            LeftArm.ResetPosePosition();
            LeftArm.Dispose();
            Brain.Dispose();
        }
    }
}
