﻿using ArmController.Models;
using ArmController.Models.Command;
using Hamsa.Common;
using Hamsa.Device;
using Hamsa.REST;
using Microsoft.Rest;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            Pause,
            Waiting,
            WaitingTouch,
            WaitingProb,
            TaskDone,
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

        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private CloudBlobContainer container;

        public override void Setup()
        {
            CommandList = new Queue<BaseCommand>();
            CurrentStatus = Status.Idle;

            Brain = new CloudBrain(new Uri("http://10.125.169.141:8182"), new BasicAuthenticationCredentials());
            ArmId = 4396;// Brain.Arm.Register().Value;

            Eye = new Camera(0);

            Arm = new ThreeDOFArm("COM4", 115200);
            Arm.Subscript("CallBack", HandleArmCallback);
            Arm.Connect();

            storageAccount = CloudStorageAccount.Parse("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference($"{Math.Abs(ArmId)}-image");
            container.CreateIfNotExists();

            //CommandList.Enqueue(new VisionCommand());
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
                case Status.WaitingTouch:
                    WaitingTouch();
                    break;
                case Status.WaitingProb:
                    WaitingProb();
                    break;
                case Status.Pause:
                    // handle PauseCommand
                    Pausing();
                    break;
                case Status.TaskDone:
                    // handle DoneCommand
                    CompleteTask();
                    break;
                case Status.Waiting:
                default:
                    Thread.Yield();
                    break;
            }
        }

        public void GetNewCommands()
        {
            var newCommandString = string.Empty;
            // get new command
            if (!IsProbed)
            {
                // if need to prob
                newCommandString = Brain.Arm.Prob(ArmId, 0);
            }
            else if (!IsCalibrated)
            {
                // if need to calibrate
                newCommandString = Brain.Arm.StartCalibrate(ArmId);
                IsCalibrated = true;
            }
            else
            {
                // get new commands
                newCommandString = Brain.Arm.GetNextTask(ArmId);
            }

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
                case CommandType.Vision:
                    var vcommand = CurrentCommand as VisionCommand;
                    HandleVisionTask(vcommand);
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
                case CommandType.Pause:
                    lock (SyncRoot)
                    {
                        CurrentStatus = Status.Pause;
                    }
                    break;
                case CommandType.Done:
                    lock (SyncRoot)
                    {
                        CurrentStatus = Status.TaskDone;
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
            if (CurrentCommand is WaitTouchCommand)
            {
                var command = CurrentCommand as WaitTouchCommand;

                if (!command.StartExecutionTime.HasValue)
                {
                    command.StartExecutionTime = DateTime.Now;
                }
                var startTime = command.StartExecutionTime.Value;
                var endTime = startTime.AddSeconds(command.TimeOutSeconds);

                if (DateTime.Now < endTime)
                {
                    var isTouchDetected = Brain.Arm.CanResume(ArmId);

                    if (!string.IsNullOrEmpty(isTouchDetected))
                    {
                        lock (SyncRoot)
                        {
                            CurrentStatus = Status.Idle;
                            CurrentCommand = null;
                        }
                    }
                    else
                    {
                        Thread.Sleep(command.RefreshIntervalMilliseconds);
                        Thread.Yield();
                    }
                }
                else
                {
                    // TimeOut
                    lock (SyncRoot)
                    {
                        CurrentStatus = Status.Idle;
                        CurrentCommand = null;
                    }
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
            if (CurrentCommand is WaitProbCommand)
            {
                var command = CurrentCommand as WaitProbCommand;

                if (!command.StartExecutionTime.HasValue)
                {
                    command.StartExecutionTime = DateTime.Now;
                }
                var startTime = command.StartExecutionTime.Value;
                var endTime = startTime.AddSeconds(command.TimeOutSeconds);

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
                        Thread.Sleep(command.RefreshIntervalMilliseconds);
                        Thread.Yield();
                    }
                }
                else
                {
                    // TimeOut, try next prob position
                    var newCommandString = Brain.Arm.Prob(
                        ArmId,
                        command.ProbRetry + 1);

                    if (!string.IsNullOrEmpty(newCommandString))
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                        var newCommands = JsonConvert.DeserializeObject<List<BaseCommand>>(newCommandString, settings);

                        foreach (var c in newCommands)
                        {
                            CommandList.Enqueue(c);
                        }
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

        public void Pausing()
        {
            if (CurrentStatus == Status.Pause)
            {
                var command = CurrentCommand as PauseCommand;

                Thread.Sleep((int)command.TimeOutMilliseconds);
            }

            lock (SyncRoot)
            {
                CurrentStatus = Status.Idle;
                CurrentCommand = null;
            }
        }

        public void CompleteTask()
        {
            if (CurrentCommand is DoneCommand)
            {
                var command = CurrentCommand as DoneCommand;

                Brain.Arm.Done(ArmId, command.RetrunData);
            }

            // current command is wrong, reset
            lock (SyncRoot)
            {
                CurrentStatus = Status.Idle;
                CurrentCommand = null;
            }
        }

        public void HandleVisionTask(VisionCommand vcommand)
        {

            if (Eye != null)
            {
                // TODO: Start Camear, Query a frame, Corp, Send to Server
                Eye.Start();
                var img = Eye.GetLatestData();

                var converter = new ImageConverter();
                byte[] byteData = (byte[])converter.ConvertTo(img, typeof(byte[]));

                var fileName = $"{Guid.NewGuid().ToString()}.jpg";
                var blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.UploadFromByteArray(byteData, 0, byteData.Length);

                Brain.Vision.Analyze(ArmId, fileName, "4396");


                Eye.Stop();
            }
            lock (SyncRoot)
            {
                CurrentCommand = null;
                CurrentStatus = Status.Idle;
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
            Brain.Dispose();
            Eye.Dispose();
            Arm.Dispose();
        }
    }
}
