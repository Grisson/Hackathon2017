using ArmController.Models.Command;
using Hamsa.Common;
using Hamsa.Device;
using Hamsa.REST;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Hamsa.UI.Code
{
    public class TestMachine : BaseExecutableCode
    {
        //[Flags]
        public enum Status
        {
            Idle,
            Waiting,
            WaitingArm,
            WaitingTouch,
        }

        public Camera Eye;
        public ThreeDOFArm Arm;

        public long ArmId;
        public CloudBrain Brain;

        public Queue<BaseCommand> CommandList;
        public BaseCommand CurrentCommand;
        public Status CurrentStatus;
        public bool IsCalibrated = false;
        public bool IsProbed = false;

        public override void Setup()
        {
            CommandList = new Queue<BaseCommand>();
            CurrentStatus = Status.Idle;

            Brain = new CloudBrain(new Uri("http://localhost:8182"), new BasicAuthenticationCredentials());
            ArmId = Brain.Arm.Register().Value;

            Eye = new Camera(0);

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
                        ExecuteCommand();
                    }
                    else
                    {
                        // get new command
                        if (!IsProbed)
                        {
                            // if need to prob
                        }
                        else if (!IsCalibrated)
                        {
                            // if need to calibrate
                            var commands = Brain.Arm.StartCalibrate(ArmId);
                        }
                        else
                        {
                            // get new commands
                        }
                    }
                    break;
                case Status.WaitingArm:
                    Thread.Yield();
                    break;
                case Status.WaitingTouch:
                    WaitingTouch();
                    break;
                case Status.Waiting:
                default:
                    Thread.Yield();
                    break;
            }
        }

        public void ExecuteCommand()
        {

        }

        public void WaitingTouch()
        {
            if (CurrentCommand is ProbWaitingCommand)
            {
                var command = CurrentCommand as ProbWaitingCommand;
                var now = DateTime.Now;
                var endTime = now.AddSeconds(command.TimeOut);

                if (DateTime.Now < endTime)
                {
                    // TODO: rename this to WaitTouch
                    var isTouchDetected = Brain.Arm.WaitProb(ArmId) ?? false;

                    if (isTouchDetected)
                    {
                        lock (SyncRoot)
                        {
                            CurrentStatus = Status.Idle;
                            CurrentCommand = null;
                        }
                    }
                    else
                    { 
                        Thread.Sleep(command.RefreshInterval);
                        Thread.Yield();
                    }
                }
            }
            else
            {
                lock(SyncRoot)
                {
                    CurrentStatus = Status.Idle;
                    CurrentCommand = null;
                }
            }
        }

        public void HandleArmCallback(string data)
        {
            if (CurrentCommand is GCommand)
            {
                var command = CurrentCommand as GCommand;
                Brain.Arm.ReportPose(ArmId,
                    command.SendTimeStamp.ToString(),
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
            Brain.Dispose();
            Eye.Dispose();
            Arm.Dispose();
        }
    }
}
