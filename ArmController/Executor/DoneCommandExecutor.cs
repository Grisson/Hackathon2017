using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }
    }
}
