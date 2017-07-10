using System;
using System.Runtime.Serialization;

namespace ArmController.Models.Command
{
    [DataContract]
    public class WaitTouchCommand : BaseCommand
    {
        [DataMember]
        public long TimeOutSeconds { get; set; }
        [DataMember]
        public int RefreshIntervalMilliseconds { get; set; }

        [IgnoreDataMember]
        public DateTime? StartExecutionTime { get; set; }

        public WaitTouchCommand() : base()
        {
            this.Type = CommandType.WaitingForTouch;

        }

        public WaitTouchCommand(int timeoutSeconds, int refreshMilliseconds) : this()
        {
            TimeOutSeconds = timeoutSeconds;
            RefreshIntervalMilliseconds = refreshMilliseconds;
        }
    }
}
