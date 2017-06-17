using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Models.Command
{
    public class WaitTouchCommand : BaseCommand
    {
        public long TimeOutMilliseconds { get; set; }
        public int RefreshInterval { get; set; }

        public WaitTouchCommand() : base()
        {
            this.Type = CommandType.WaitingForTouch;

        }

        public WaitTouchCommand(long timeoutMS, int refreshMilliseconds) : this()
        {
            TimeOutMilliseconds = timeoutMS;
            RefreshInterval = refreshMilliseconds;
        }
    }
}
