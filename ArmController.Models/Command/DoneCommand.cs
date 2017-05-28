namespace ArmController.Models.Command
{
    public class DoneCommand : BaseCommand
    {
        public string URL { get; set; }

        public string RetrunData { get; set; }

        public DoneCommand(string data)
        {
            this.Type = CommandType.Done;
            RetrunData = data;
        }

        public override string ToSendLog
        {
            get
            {
                return $"Done Command";
            }
        }
    }
}
