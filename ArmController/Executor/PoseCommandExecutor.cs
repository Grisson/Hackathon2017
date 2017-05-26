using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Executor
{
    public class PoseCommandExecutor : IExecutor
    {
        public static readonly PoseCommandExecutor SharedInstance = new PoseCommandExecutor();

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

        private PoseCommandExecutor()
        {

        }

        public void Execute(BaseCommand command)
        {
            this.Execute(command as PoseCommand);
        }

        public void Execute(PoseCommand command)
        {
            if ((SerialPort == null) || !SerialPort.IsConnected)
            {
                return;
            }

            command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // command text is based on target pose and current pose
            SerialPort.WriteLine(command.CommandText);
        }

        public void Callback(SerialPort sender, PoseCommand command)
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
                        CommandExecutor.SharedInstance.TestBrain.ReportAgentPosePosition(command.SendTimeStamp,
                            CommandStore.SharedInstance.CurrentPosePosition.X,
                            CommandStore.SharedInstance.CurrentPosePosition.Y,
                            CommandStore.SharedInstance.CurrentPosePosition.Z);

                        LogHandler?.Invoke($"Reported Pose: {CommandStore.SharedInstance.CurrentPosePosition.X}, {CommandStore.SharedInstance.CurrentPosePosition.Y}, {CommandStore.SharedInstance.CurrentPosePosition.Z}");

                        // clean the current command
                        CommandStore.SharedInstance.CurrentCommand = null;

                        // clean the waiting flag
                        lock (CommandExecutor.SharedInstance)
                        {
                            IsWaitingResponse = false;
                        }
                    }
                }
            }
        }
    }
}
