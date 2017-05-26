using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (command.ResetPosition)
            {
                command.XDelta = command.CurrentPosePosition.X * -1;
                command.YDelta = command.CurrentPosePosition.Y * -1;
                command.ZDelta = command.CurrentPosePosition.Z * -1;
                command.RefreshCommandText();
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
