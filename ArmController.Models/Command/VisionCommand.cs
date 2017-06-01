namespace ArmController.Models.Command
{
    public class VisionCommand : BaseCommand
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }


        public string Data { get; set; }

        public VisionCommand()
        {
            this.Type = CommandType.Vision;
        }

        public VisionCommand(string data) : this()
        {
            this.Data = data;
        }

        public override string ToSendLog
        {
            get
            {
                return $"Screen Shot Command";
            }
        }
    }
}
