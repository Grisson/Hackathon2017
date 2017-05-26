using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib.Data
{
    public class ProbCommand : BaseCommand
    {
        public PosePosition initPose = new PosePosition(2500, 2500, 0);
        public int ProbInterval = 2;

        public ProbCommand() : base()
        {
            this.Type = CommandType.Prob;
        }
    }
}
