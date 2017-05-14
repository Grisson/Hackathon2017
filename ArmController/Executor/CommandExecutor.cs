using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController
{
    public class CommandExecutor
    {
        public static readonly CommandExecutor SharedInstance = new CommandExecutor();

        public bool IsWaitingResponse { get; set; }

        private CommandExecutor()
        {

        }

        public void Execute(BaseCommand command)
        {
            switch(command.Type)
            {
                case CommandType.GCode:
                    this.Execute(command as GCommand);
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

        public void Execute(GCommand command)
        {

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
