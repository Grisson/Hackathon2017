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
        public ThreeDOFArm Arm;

        public long ArmId;
        public CloudBrain Brain;

        public Queue<BaseCommand> CommandList;
        public BaseCommand CurrentCommand;
        public Status CurrentStatus;

        public override void Setup()
        {
            CommandList = new Queue<BaseCommand>();
            CurrentStatus = Status.Idle;

            Brain = new CloudBrain(new Uri("http://10.125.169.141:8182"), new BasicAuthenticationCredentials());
            ArmId = 4396;// Brain.Arm.Register().Value;


            Arm = new ThreeDOFArm("COM4", 115200);
            Arm.Subscript("CallBack", HandleArmCallback);
            Arm.Connect();
        }

        public override void Loop()
        {
            switch (CurrentStatus)
            {
                case Status.Idle:
                    if (CommandList.Count > 0)
                    {
                        ExecuteNextCommand();
                    }
                    else
                    {
                        GetNewCommands();
                    }
                    break;
                case Status.Executing:
                    break;
                default:
                    Thread.Yield();
                    break;
            }
        }

        public void GetNewCommands()
        {
            // get new command
            var newCommandString = Brain.Arm.GetNextTask(ArmId);

            if (string.IsNullOrEmpty(newCommandString))
            {
                return;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var newCommands = JsonConvert.DeserializeObject<List<BaseCommand>>(newCommandString, settings);

            foreach (var c in newCommands)
            {
                CommandList.Enqueue(c);
            }
        }

        public void ExecuteNextCommand()
        {
            lock (SyncRoot)
            {
                CurrentCommand = CommandList.Dequeue();
                CurrentStatus = Status.Executing;
            }

            switch (CurrentCommand.Type)
            {
                case CommandType.Pose:
                    var command = CurrentCommand as PoseCommand;
                    command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    Arm.MoveTo(new PosePosition() { MotorOneSteps = command.NextPosePosition.X, MotorTwoSteps = command.NextPosePosition.Y, MotorThreeSteps = command.NextPosePosition.Z });
                    break;
                case CommandType.GCode:
                    var gcommand = CurrentCommand as GCommand;
                    gcommand.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var newPose = new PosePosition()
                    {
                        MotorOneSteps = Arm.CurrentPose.X + gcommand.XDelta,
                        MotorTwoSteps = Arm.CurrentPose.Y + gcommand.YDelta,
                        MotorThreeSteps = Arm.CurrentPose.Z + gcommand.ZDelta,
                    };
                    Arm.MoveTo(newPose);
                    break;
                
                default:
                    lock (SyncRoot)
                    {
                        CurrentCommand = null;
                        CurrentStatus = Status.Idle;
                    }
                    break;
            }
        }

        public void HandleArmCallback(string data)
        {
            if (CurrentStatus == Status.Executing)
            {
                var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (CurrentCommand is GCommand)
                {
                    var gcommand = CurrentCommand as GCommand;
                    timeStamp = gcommand.SendTimeStamp;
                }
                else if (CurrentCommand is PoseCommand)
                {
                    var gcommand = CurrentCommand as PoseCommand;
                    timeStamp = gcommand.SendTimeStamp;
                }
                Brain.Arm.ReportPose(ArmId,
                    timeStamp.ToString(),
                    Arm.CurrentPose.MotorOneSteps,
                    Arm.CurrentPose.MotorTwoSteps,
                    Arm.CurrentPose.MotorThreeSteps);

                lock (SyncRoot)
                {
                    CurrentCommand = null;
                    CurrentStatus = Status.Idle;
                }
            }
        }

        public override void Cleanup()
        {
            Arm.ResetPosePosition();
            Arm.Dispose();
            Brain.Dispose();
        }
    }
}
