using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArmController.Executor
{
    public class CommandExecutor
    {
        private CommandStore _commands => CommandStore.SharedInstance;

        public bool IsWaitingResponse { get; set; }

        public static readonly CommandExecutor SharedInstance = new CommandExecutor();

        private CommandExecutor()
        {
        }

        /// <summary>
        /// Dequeue command from command store
        /// </summary>
        public void Execute()
        {
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
                return;
            }

            BaseCommand _currentCommand = null;
            if (_commands.TryDequeue(out _currentCommand))
            {
                if(_currentCommand != null)
                {
                    this.Execute(_currentCommand);
                }
                else
                {
                    lock (CommandExecutor.SharedInstance)
                    {
                        IsWaitingResponse = false;
                    }
                }

                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    _dataContext.AddOutput(tmpCommand.ToSendLog());
                //});
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
                    this.Execute(command as PauseCommand);
                    break;
                case CommandType.Resume:
                    this.Execute(command as ResumeCommand);
                    break;
                case CommandType.ScreenShot:
                    this.Execute(command as ScreenShotCommand);
                    break;
                default:
                    break;
            }
        }

        public void Execute(PauseCommand command)
        {

        }

        public void Execute(ResumeCommand command)
        {

        }

        public void Execute(ScreenShotCommand command)
        {

        }
    }
}
