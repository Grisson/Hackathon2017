using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib.Data
{
    public enum CommandType
    {
        GCode = 0,
        Pause,
        Resume,
        WaitingForTouch,
        ScreenShot,
        Done,
    }

    public class BaseCommand
    {
        public Guid CommandHistoryId { get; set; }
        public CommandType Type { get; set; }

        public BaseCommand()
        {
            CommandHistoryId = Guid.NewGuid();
        }

        public virtual string ToSendLog { get; }
    }
}
