namespace ArmController.Models.Command
{
    public class ScreenShotCommand : BaseCommand
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }


        public ScreenShotCommand()
        {
            this.Type = CommandType.ScreenShot;
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
