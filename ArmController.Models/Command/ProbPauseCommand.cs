namespace ArmController.Models.Command
{
    public class ProbWaitingCommand : BaseCommand
    {
        public int ProbRetry = 0;
        public long TimeOutSeconds { get; set; }
        public int RefreshInterval { get; set; }

        public ProbWaitingCommand() : base()
        {
            this.Type = CommandType.WaitingProb;
        }

        public ProbWaitingCommand(long timeout, int refresh, int retry) : this()
        {
            TimeOutSeconds = timeout;
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
