using ArmController.Models;
using ArmController.Models.Command;
using ArmController.REST;
using Microsoft.Rest;
using System;
using System.Configuration;
using System.IO.Ports;

namespace ArmController.Executor
{
    public class CommandExecutor
    {
        public static readonly CommandExecutor SharedInstance = new CommandExecutor();

        public readonly CloudBrain brain;

        public long? RegisterId;

        public bool IsRegisted => RegisterId.HasValue;

        public SerialCommunicator SerialPort { get; set; }

        public bool IsWaitingResponse { get; set; }

        public bool IsStopped { get; set; }

        private CommandStore _commands => CommandStore.SharedInstance;

        public Action<string> LogHandler;

        public Action<string> TakePhoto;

        private CommandExecutor()
        {
            var baseUrl = new Uri(ConfigurationManager.AppSettings["BaseUrl"]);
            brain = new CloudBrain(baseUrl, new BasicAuthenticationCredentials());
        }


        public void Register()
        {
            if(brain != null)
            {
                RegisterId = brain.Arm.Register();
            }
        }
        /// <summary>
        /// Dequeue command from command store
        /// </summary>
        public void Execute()
        {
            if(IsStopped)
            {
                CommandStore.SharedInstance.DeleteAll();
                IsStopped = false;
                IsWaitingResponse = false;
                return;
            }

            var continueToExcute = false;
            if (!IsWaitingResponse)
            {
                lock (CommandExecutor.SharedInstance)
                {
                    if (!IsWaitingResponse && _commands.Count > 0)
                    {
                        IsWaitingResponse = true;
                        continueToExcute = true;
                    }
                }
            }

            if (!continueToExcute)
            {
                // no need to start again here, because there is another which is executing
                return;
            }

            BaseCommand _currentCommand = null;
            if (_commands.TryDequeue(out _currentCommand))
            {
                if(_currentCommand != null)
                {
                    LogHandler?.Invoke(_currentCommand.ToSendLog);

                    this.Execute(_currentCommand);
                }
                else
                {
                    lock (CommandExecutor.SharedInstance)
                    {
                        IsWaitingResponse = false;
                    }

                    // no need to start again here, because there is no command to execute
                }
            }
            else
            {
                lock (CommandExecutor.SharedInstance)
                {
                    IsWaitingResponse = false;
                }
            }
        }

        /// <summary>
        /// Decide which executor to call
        /// </summary>
        /// <param name="command"></param>
        public void Execute(BaseCommand command)
        {
            switch(command.Type)
            {
                case CommandType.GCode:
                    GCommandExecutor.SharedInstance.Execute(command);
                    break;
                case CommandType.Pause:
                    PauseCommandExecutor.SharedInstance.Execute(command);
                    break;
                case CommandType.Resume:
                    //this.Execute(command as ResumeCommand);
                    break;
                case CommandType.Vision:
                    VisionCommandExecutor.SharedInstance.Execute(command);
                    break;
                case CommandType.Done:
                    DoneCommandExecutor.SharedInstance.Execute(command);
                    break;
                case CommandType.Pose:
                    PoseCommandExecutor.SharedInstance.Execute(command);
                    break;
                case CommandType.WaitingProb:
                    ProbPauseCommandExecutor.SharedInstance.Execute(command);
                    break;
                default:
                    break;
            }
        }

        public void Callback(object sender, String data, BaseCommand command)
        {
            switch (command.Type)
            {
                case CommandType.GCode:
                    GCommandExecutor.SharedInstance.Callback(sender as SerialPort, command as GCommand);
                    break;
                case CommandType.Pose:
                    PoseCommandExecutor.SharedInstance.Callback(sender as SerialPort, command as PoseCommand);
                    break;
                default:
                    break;
            }
        }
    }
}
