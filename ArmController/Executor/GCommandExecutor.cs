﻿using ArmController.Models.Command;
using ArmController.REST;
using System;
using System.IO.Ports;

namespace ArmController.Executor
{
    public class GCommandExecutor : IExecutor
    {
        public static readonly GCommandExecutor SharedInstance = new GCommandExecutor();

        public SerialCommunicator SerialPort => CommandExecutor.SharedInstance.SerialPort;

        public Action<string> LogHandler => CommandExecutor.SharedInstance.LogHandler;

        private bool IsWaitingResponse
        {
            get
            {
                return CommandExecutor.SharedInstance.IsWaitingResponse;
            }
            set
            {
                CommandExecutor.SharedInstance.IsWaitingResponse = value;
            }
        }

        private GCommandExecutor()
        {

        }

        public void Execute(BaseCommand command)
        {
            this.Execute(command as GCommand);
        }

        public void Execute(GCommand command)
        {
            if ((SerialPort == null) || !SerialPort.IsConnected)
            {
                return;
            }

            if (command.CurrentPosePosition == null)
            {
                command.CurrentPosePosition = CommandStore.SharedInstance.CurrentPosePosition;
            }

            command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            SerialPort.WriteLine(command.CommandText);
        }

        public void Callback(SerialPort sender, GCommand command)
        {
            var sp = sender;
            while (sp.BytesToRead > 0)
            {
                var d = sp.ReadLine();

                if (command != null)
                {
                    command.Receive(d);
                    LogHandler?.Invoke(command.ToReceiveLog());
                    // if the gcommand is executed
                    if (d.Equals("OK\r", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // update the current position
                        if (command.NextPosePosition != null)
                        {
                            CommandStore.SharedInstance.CurrentPosePosition = command.NextPosePosition;
                        }

                        // report current position
                        CommandExecutor.SharedInstance.brain.Arm.ReportPose(
                            CommandExecutor.SharedInstance.RegisterId.Value,
                            command.SendTimeStamp.ToString(),
                            CommandStore.SharedInstance.CurrentPosePosition.X,
                            CommandStore.SharedInstance.CurrentPosePosition.Y,
                            CommandStore.SharedInstance.CurrentPosePosition.Z);

                        LogHandler?.Invoke($"Reported Pose: {CommandStore.SharedInstance.CurrentPosePosition.X}, {CommandStore.SharedInstance.CurrentPosePosition.Y}, {CommandStore.SharedInstance.CurrentPosePosition.Z}");

                        lock (CommandExecutor.SharedInstance)
                        {
                            // clean the current command
                            CommandStore.SharedInstance.CurrentCommand = null;
                            // clean the waiting flag
                            IsWaitingResponse = false;
                        }
                    }
                }
            }
        }
    }
}
