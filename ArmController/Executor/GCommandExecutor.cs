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

        public SerialCommunicator SerialPort { get; set; }

        private CommandStore _commands => CommandStore.SharedInstance;

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
            command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            SerialPort.WriteLine(command.CommandText);
        }
    }
}
