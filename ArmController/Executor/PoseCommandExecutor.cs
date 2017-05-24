using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Executor
{
    public class PoseCommandExecutor : IExecutor
    {
        public static readonly PoseCommandExecutor SharedInstance = new PoseCommandExecutor();

        public SerialCommunicator SerialPort => CommandExecutor.SharedInstance.SerialPort;

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

            if (command.CurrentPosePosition == null)
            {
                command.CurrentPosePosition = CommandStore.SharedInstance.CurrentPosePosition;
            }

            command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            SerialPort.WriteLine(command.CommandText);
        }
    }
}
