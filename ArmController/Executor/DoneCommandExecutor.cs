using ArmController.lib.Data;
using ArmController.Models.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmController.Executor
{
    public class DoneCommandExecutor
    {
        public static readonly DoneCommandExecutor SharedInstance = new DoneCommandExecutor();
        public Action<string> LogHandler => CommandExecutor.SharedInstance.LogHandler;

        private DoneCommandExecutor()
        {

        }

        public void Execute(BaseCommand command)
        {
            this.Execute(command as DoneCommand);
        }

        public void Execute(DoneCommand command)
        {
            CommandExecutor.SharedInstance.TestBrain.Done(command.RetrunData);
            LogHandler?.Invoke($"DoneCommand({command.RetrunData}) is executed!");

            lock (CommandExecutor.SharedInstance)
            {
                CommandStore.SharedInstance.CurrentCommand = null;
                CommandExecutor.SharedInstance.IsWaitingResponse = false;
            }

            new Thread(() => {
                LogHandler?.Invoke($"Will sleep 500ms for safty");
                Thread.Sleep(500);
                CommandExecutor.SharedInstance.Execute();
            }).Start();
        }
    }
}
