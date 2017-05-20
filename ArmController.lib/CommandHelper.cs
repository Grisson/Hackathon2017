using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib
{
    public static class CommandHelper
    {
        public static double LiftUpDistance => 2;
        public static double MMToSteps = 250;


        public static GCommand LiftUp()
        {
            return new GCommand(-1 * LiftUpDistance * MMToSteps, -1 * LiftUpDistance * MMToSteps, 0);
        }

        public static GCommand TouchDown()
        {
            return new GCommand(LiftUpDistance * MMToSteps, LiftUpDistance * MMToSteps, 0);
        }

        public static List<BaseCommand> Tap()
        {
            return new List<BaseCommand> { TouchDown(), LiftUp() };
        }

        public static GCommand ChangeLength(double dist)
        {
            return new GCommand(dist, -1 * dist, 0);
        }

        public static List<BaseCommand> TouchPointsInSameRadius(List<double> zInternals)
        {
            return TouchPointsInSameRadius(zInternals, 0);
        }

        public static List<BaseCommand> TouchPointsInSameRadius(List<double> zInternals, int repeatTimes)
        {
            var result = new List<BaseCommand>();

            foreach (var z in zInternals)
            {
                result.Add(new GCommand(0, 0, z));

                result.AddRange(Tap());
                result.Add(new PauseCommand(30, 500));

                for (var i = 1; i <= repeatTimes; i++)
                {
                    result.AddRange(Tap());
                    result.Add(new PauseCommand(30, 500));
                }
            }

            return result;
        }
    }
}
