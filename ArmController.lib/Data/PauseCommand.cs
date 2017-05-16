using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib.Data
{
    public class PauseCommand : BaseCommand
    {
        public long TimeOut { get; set; }
        public int RefreshInterval { get; set; }


        public override string ToSendLog
        {
            get
            {
                return $"Pause Command";
            }
        }
    }
}
