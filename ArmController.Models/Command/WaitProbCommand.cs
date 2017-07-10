using System;
using System.Runtime.Serialization;

namespace ArmController.Models.Command
{
    [DataContract]
    public class WaitProbCommand : BaseCommand
    {
        [DataMember]
        public int ProbRetry = 0;
        [DataMember]
        public long TimeOutSeconds { get; set; }
        [DataMember]
        public int RefreshIntervalMilliseconds { get; set; }

        [IgnoreDataMember]
        public DateTime? StartExecutionTime { get; set; }

        public WaitProbCommand() : base()
        {
            this.Type = CommandType.WaitingProb;
        }

        public WaitProbCommand(long timeoutSeconds, int refreshMilliseconds, int retry) : this()
        {
            TimeOutSeconds = timeoutSeconds;
            RefreshIntervalMilliseconds = refreshMilliseconds;
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
