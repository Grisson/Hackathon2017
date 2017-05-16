using ArmController.lib.Data;
using Newtonsoft.Json;
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
                while(DateTime.Now < endTime)
                {
                    if (command.RefreshInterval > 0)
                    {
                        // do something
                        var retrunCommand = CommandExecutor.SharedInstance.TestBrain.CanResume();

                        if(!string.IsNullOrEmpty(retrunCommand))
                        {
                            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                            var resumeCommand = JsonConvert.DeserializeObject<ResumeCommand>(retrunCommand, settings);
                            break;
                        }
                        else
                        {
                            Thread.Sleep(command.RefreshInterval);
                        }
                    }
                    else
                    {
                        Thread.Sleep(timeOutTimeSpan);
                    }
                }
            }
            finally
            {
                // unlock
                lock (CommandExecutor.SharedInstance)
                {
                    CommandStore.SharedInstance.CurrentCommand = null;
                    CommandExecutor.SharedInstance.IsWaitingResponse = false;
                }

                new Thread(() => {
                    Thread.Sleep(2000);
                    CommandExecutor.SharedInstance.Execute();
                }).Start();


            }
        }
    }
}
