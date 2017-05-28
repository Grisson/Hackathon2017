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
