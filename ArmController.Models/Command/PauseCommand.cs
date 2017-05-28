namespace ArmController.Models.Command
{
    public class PauseCommand : BaseCommand
    {
        public long TimeOut { get; set; }
        public int RefreshInterval { get; set; }
        public int RegreshURL { get; set; }

        public PauseCommand()
        {
            this.Type = CommandType.Pause;
        }

        public PauseCommand(long timeout, int refresh) : this()
        {
            TimeOut = timeout;
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
