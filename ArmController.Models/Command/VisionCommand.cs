using System.Runtime.Serialization;

namespace ArmController.Models.Command
{
    [DataContract]
    public class VisionCommand : BaseCommand
    {
        [DataMember]
        public double X { get; set; }
        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public double Width { get; set; }
        [DataMember]
        public double Height { get; set; }


        [DataMember]
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
