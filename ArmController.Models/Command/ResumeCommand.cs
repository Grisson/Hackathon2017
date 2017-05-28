using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Models.Command
{
    public class ResumeCommand : BaseCommand
    {
        public ResumeCommand()
        {
            this.Type = CommandType.Resume;
        }

        public override string ToSendLog
        {
            get
            {
                return $"Pause Command";
            }
        }
    }
}
