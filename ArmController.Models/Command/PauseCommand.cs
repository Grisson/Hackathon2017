using System;
using System.Runtime.Serialization;

namespace ArmController.Models.Command
{
    public class PauseCommand : BaseCommand
    {
        public long TimeOutMilliseconds { get; set; }
        public int RefreshInterval { get; set; }
        public int RegreshURL { get; set; }

        [IgnoreDataMember]
        public DateTime? StartExecutionTime { get; set; }

        public PauseCommand() : base()
        {
            this.Type = CommandType.Pause;
        }

        public PauseCommand(long timeoutMS, int refresh) : this()
        {
            TimeOutMilliseconds = timeoutMS;
            RefreshInterval = refresh;
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
