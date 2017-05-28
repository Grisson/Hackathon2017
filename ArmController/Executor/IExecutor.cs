using ArmController.lib.Data;
using ArmController.Models.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Executor
{
    interface IExecutor
    {
        void Execute(BaseCommand command);
    }
}
