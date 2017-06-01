using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Models
{
    public enum CommandType
    {
        GCode = 0,
        Pause,
        Resume,
        WaitingForTouch,
        Vision,
        Done,
        Pose,
        Prob,
    }
}
