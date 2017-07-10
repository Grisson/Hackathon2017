using System.Runtime.Serialization;

namespace ArmController.Models.Command
{
    [DataContract]
    public class DoneCommand : BaseCommand
    {
        [DataMember]
        public string URL { get; set; }

        [DataMember]
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
