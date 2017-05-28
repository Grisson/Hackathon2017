using ArmController.lib.Data;
using ArmController.Models.Command;
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

        public Action<string> LogHandler => CommandExecutor.SharedInstance.LogHandler;

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
            LogHandler?.Invoke("Start to Execute Pause Command");
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
                            LogHandler?.Invoke("Will resume!");
                            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                            var resumeCommand = JsonConvert.DeserializeObject<ResumeCommand>(retrunCommand, settings);
                            break;
                        }
                        else
                        {
                            LogHandler?.Invoke($"Will sleep {command.RefreshInterval}ms");
                            Thread.Sleep(command.RefreshInterval);
                        }
                    }
                    else
                    {
                        LogHandler?.Invoke($"Will sleep {timeOutTimeSpan.Seconds}s");
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
                    LogHandler?.Invoke($"Will sleep 500ms for safty");
                    Thread.Sleep(500);
                    CommandExecutor.SharedInstance.Execute();
                }).Start();
            }
        }
    }
}
