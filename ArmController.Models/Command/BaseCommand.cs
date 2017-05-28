using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Models.Command
{
    public class BaseCommand
    {
        public Guid CommandHistoryId { get; set; }
        public CommandType Type { get; set; }

        public BaseCommand()
        {
            CommandHistoryId = Guid.NewGuid();
        }

        public virtual string ToSendLog { get; }
    }
}
