using System;
using System.Runtime.Serialization;

namespace ArmController.Models.Command
{
    [DataContract]
    public class BaseCommand
    {
        [DataMember]
        public Guid CommandHistoryId { get; set; }

        [DataMember]
        public CommandType Type { get; set; }

        public BaseCommand()
        {
            CommandHistoryId = Guid.NewGuid();
        }

        public virtual string ToSendLog { get; }
    }
}
