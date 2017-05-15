using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmController.Executor
{
    public class PauseCommandExecutor : IExecutor
    {
        public static readonly PauseCommandExecutor SharedInstance = new PauseCommandExecutor();

        private PauseCommandExecutor()
        {

        }

        public void Execute(BaseCommand command)
        {
            this.Execute(command as PauseCommand);
        }

        public void Execute(PauseCommand command)
        {
            var now = DateTime.Now;
            var endTime = now.AddSeconds(command.TimeOut);
            var timeOutTimeSpan = endTime - now;

            try
            {
                if(command.RefreshInterval > 0)
                {
                    // do something
                }
                else
                {
                    Thread.Sleep(timeOutTimeSpan);
                }
            }
            finally
            {
                // unlock
                lock (CommandExecutor.SharedInstance)
                {
                    CommandExecutor.SharedInstance.IsWaitingResponse = false;
                }
            }
        }
    }
}
