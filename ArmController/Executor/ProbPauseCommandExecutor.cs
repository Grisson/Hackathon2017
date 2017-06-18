using ArmController.Models.Command;
using ArmController.REST;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ArmController.Executor
{
    public class ProbPauseCommandExecutor : IExecutor
    {
        public static readonly ProbPauseCommandExecutor SharedInstance = new ProbPauseCommandExecutor();

        private Queue<BaseCommand> localCommandQueue = new Queue<BaseCommand>();

        public Action<string> LogHandler => CommandExecutor.SharedInstance.LogHandler;
        public SerialCommunicator SerialPort => CommandExecutor.SharedInstance.SerialPort;

        private ProbPauseCommandExecutor()
        {

        }

        public void Execute(BaseCommand command)
        {
            this.Execute(command as WaitProbCommand);
        }

        public void Execute(WaitProbCommand command)
        {
            var now = DateTime.Now;
            var endTime = now.AddSeconds(command.TimeOutSeconds);
            var timeOutTimeSpan = endTime - now;
            LogHandler?.Invoke("Start to Execute Prob Pause Command");
            var isTouchDetected = false;
            try
            {
                while (DateTime.Now < endTime)
                {
                    if (command.RefreshIntervalMilliseconds > 0)
                    {
                        // do something
                        isTouchDetected = CommandExecutor.SharedInstance.brain.Arm.WaitProb(
                            CommandExecutor.SharedInstance.RegisterId.Value) ?? false;

                        if (isTouchDetected)
                        {
                            LogHandler?.Invoke("Will resume!");
                            break;
                        }
                        else
                        {
                            LogHandler?.Invoke($"no touch detected!");
                            LogHandler?.Invoke($"Will sleep {command.RefreshIntervalMilliseconds}ms");
                            Thread.Sleep(command.RefreshIntervalMilliseconds);
                        }
                    }
                    else
                    {
                        LogHandler?.Invoke($"Will sleep {timeOutTimeSpan.Seconds}s");
                        Thread.Sleep(timeOutTimeSpan);
                    }
                }

                var newCommandString = string.Empty;
                if(isTouchDetected)
                {
                    newCommandString = CommandExecutor.SharedInstance.brain.Arm.StartCalibrate(
                        CommandExecutor.SharedInstance.RegisterId.Value);
                }
                else
                {
                    newCommandString = CommandExecutor.SharedInstance.brain.Arm.Prob(
                        CommandExecutor.SharedInstance.RegisterId.Value,
                        command.ProbRetry + 1); 
                }

                if(string.IsNullOrEmpty(newCommandString))
                {
                    return;
                }

                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                var newCommands = JsonConvert.DeserializeObject<List<BaseCommand>>(newCommandString, settings);

                foreach(var c in newCommands)
                {
                    CommandStore.SharedInstance.Enqueue(c);
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
