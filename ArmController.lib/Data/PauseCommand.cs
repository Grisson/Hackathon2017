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
        public long RefreshInterval { get; set; }
    }
}
