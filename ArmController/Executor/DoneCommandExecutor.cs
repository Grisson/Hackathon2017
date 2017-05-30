using ArmController.Models.Command;
using ArmController.REST;
using System;
using System.Threading;

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
            CommandExecutor.SharedInstance.brain.Arm.Done(CommandExecutor.SharedInstance.RegisterId.Value, command.RetrunData);
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
