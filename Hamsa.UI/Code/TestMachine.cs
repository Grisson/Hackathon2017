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
    public class TestMachine : BaseExecutableCode
    {
        //[Flags]
        public enum Status
        {
            Idle,
            Executing,
            Waiting,
            WaitingTouch,
            WaitingProb,
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
                        ExecuteNextCommand();
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
                case Status.WaitingTouch:
                    WaitingTouch();
                    break;
                case Status.WaitingProb:
                    WaitingProb();
                    break;
                case Status.Waiting:
                default:
                    Thread.Yield();
                    break;
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
                case CommandType.GCode:
                    var command = CurrentCommand as GCommand;
                    command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    Arm.MoveTo(new PosePosition() { MotorOneSteps = command.XDelta, MotorTwoSteps = command.YDelta, MotorThreeSteps = command.ZDelta });
                    break;
                case CommandType.Vision:
                    // TODO: Start Camear, Query a frame, Corp, Send to Server
                    break;
                case CommandType.WaitingForVisionAnalyze:
                    // TODO: Check Server Status;
                    break;
                case CommandType.WaitingProb:
                    lock (SyncRoot)
                    {
                        CurrentStatus = Status.WaitingProb;
                    }
                    break;
                case CommandType.WaitingForTouch:
                    lock (SyncRoot)
                    {
                        CurrentStatus = Status.WaitingTouch;
                    }
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

        public void WaitingTouch()
        {
            // TODO: check server side 
            if (CurrentCommand is ProbWaitingCommand)
            {
                var command = CurrentCommand as ProbWaitingCommand;
                var now = DateTime.Now;
                var endTime = now.AddSeconds(command.TimeOutSeconds);

                if (DateTime.Now < endTime)
                {
                    // TODO: rename this to WaitTouch
                    //var isTouchDetected = Brain.Arm.touc.WaitProb(ArmId) ?? false;
                    var isTouchDetected = true;

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
                else
                {
                    // TimeOut
                }
            }
            else
            {
                lock (SyncRoot)
                {
                    CurrentStatus = Status.Idle;
                    CurrentCommand = null;
                }
            }
        }

        public void WaitingProb()
        {
            if (CurrentCommand is ProbWaitingCommand)
            {
                var command = CurrentCommand as ProbWaitingCommand;
                var now = DateTime.Now;
                var endTime = now.AddSeconds(command.TimeOutSeconds);

                if (DateTime.Now < endTime)
                {
                    // check if prob rename this to WaitTouch
                    var isTouchDetected = Brain.Arm.WaitProb(ArmId) ?? false;

                    if (isTouchDetected)
                    {
                        IsProbed = true;
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
                else
                {
                    // TimeOut, try next prob position
                    var newCommandString = Brain.Arm.Prob(
                        ArmId,
                        command.ProbRetry + 1);

                    JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                    var newCommands = JsonConvert.DeserializeObject<List<BaseCommand>>(newCommandString, settings);

                    foreach (var c in newCommands)
                    {
                        CommandList.Enqueue(c);
                    }

                    lock (SyncRoot)
                    {
                        CurrentStatus = Status.Idle;
                        CurrentCommand = null;
                    }
                }
            }
            else
            {
                // current command is wrong, reset
                lock (SyncRoot)
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
