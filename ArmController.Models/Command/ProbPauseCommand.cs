using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Models.Command
{
    public class ProbPauseCommand : BaseCommand
    {
        public int ProbRetry = 0;
        public long TimeOut { get; set; }
        public int RefreshInterval { get; set; }

        public ProbPauseCommand() : base()
        {
            this.Type = CommandType.Prob;
        }

        public ProbPauseCommand(long timeout, int refresh, int retry) : this()
        {
            TimeOut = timeout;
            RefreshInterval = refresh;
            ProbRetry = retry;
        }

        public override string ToSendLog
        {
            get
            {
                return $"Prob Pause Command";
            }
        }
    }
}
