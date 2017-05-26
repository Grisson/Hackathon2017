using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmController.Executor
{
    public class ProbCommandExecutor : IExecutor
    {
        public static readonly ProbCommandExecutor SharedInstance = new ProbCommandExecutor();


        private Queue<BaseCommand> localCommandQueue = new Queue<BaseCommand>();

        public Action<string> LogHandler => CommandExecutor.SharedInstance.LogHandler;
        public SerialCommunicator SerialPort => CommandExecutor.SharedInstance.SerialPort;

        private ProbCommandExecutor()
        {

        }

        public void Execute(BaseCommand command)
        {
            this.Execute(command as ProbCommand);
        }

        public void Execute(ProbCommand command)
        {
            if ((CommandStore.SharedInstance.CurrentCommand is ProbCommand) || (CommandStore.SharedInstance.Count != 0 ))
            {
                LogHandler?.Invoke("Prob cannot be executed. Stop all commands");

                CommandStore.SharedInstance.DeleteAll();
                CompleteCurrentCommand();

                return;
            }

            localCommandQueue.Clear();

            localCommandQueue.Enqueue(new GCommand() { ResetPosition = true });

        }

        public void GCommandCallBack()
        {

        }

        

        private void CompleteCurrentCommand()
        {
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
