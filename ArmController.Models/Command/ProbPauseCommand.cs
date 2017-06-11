namespace ArmController.Models.Command
{
    public class ProbWaitingCommand : BaseCommand
    {
        public int ProbRetry = 0;
        public long TimeOut { get; set; }
        public int RefreshInterval { get; set; }

        public ProbWaitingCommand() : base()
        {
            this.Type = CommandType.Prob;
        }

        public ProbWaitingCommand(long timeout, int refresh, int retry) : this()
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
