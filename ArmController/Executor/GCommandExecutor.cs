using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Executor
{
    public class GCommandExecutor
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

            //var continueToExcute = false;
            //if (!IsWaitingResponse)
            //{
            //    lock (this)
            //    {
            //        if (!IsWaitingResponse && _commands.Count > 0)
            //        {
            //            IsWaitingResponse = true;
            //            continueToExcute = true;
            //        }
            //    }
            //}

            //if (!continueToExcute)
            //{
            //    return;
            //}

            //BaseCommand _currentCommand = null;
            //if (_commands.TryDequeue(out _currentCommand))
            //{
            //    var tmpCommand = (GCommand)_currentCommand;
            command.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            SerialPort.WriteLine(command.CommandText);
            //Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        _dataContext.AddOutput(tmpCommand.ToSendLog());
            //    });

            //}
            //else
            //{
            //    lock (this)
            //    {
            //        IsWaitingResponse = false;
            //    }
            //}
        }

    }
}
