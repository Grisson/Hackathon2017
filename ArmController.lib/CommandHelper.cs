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
        public static int LiftUpDistance => 2;
        public static int MMToSteps = 250;
        public static int WaitingTimeOutSeconds = 30;
        public static int RefreshIntervalMillSeconds = 500;

        public static int GetTapDistance()
        {
            return LiftUpDistance * MMToSteps;
        }

        public static void WaitForTouch(this List<BaseCommand> commands)
        {
            commands.Add(CommandHelper.WaitForTouch());
        }

        public static void LiftUp(this List<BaseCommand> commands)
        {
            commands.Add(CommandHelper.LiftUp());
        }

        public static void TouchDown(this List<BaseCommand> commands)
        {
            commands.Add(CommandHelper.TouchDown());
        }

        public static void Tap(this List<BaseCommand> commands)
        {
            commands.AddRange(CommandHelper.Tap());
        }

        [Obsolete("Use coordinate api")]
        public static void ChangeLength(this List<BaseCommand> commands, int dist)
        {
            commands.Add(CommandHelper.ChangeLength(dist));
        }

        public static void TouchInSameRadius(this List<BaseCommand> commands, List<int> zInternals)
        {
            commands.AddRange(CommandHelper.TouchInSameRadius(zInternals));
        }

        public static void TouchPointsInSameRadius(this List<BaseCommand> commands, List<int> zInternals, int repeatTimes)
        {
            commands.AddRange(CommandHelper.TouchInSameRadius(zInternals, repeatTimes));
        }

        public static void TouchInStairs(this List<BaseCommand> commands, List<int> lengths, List<int> zInternals)
        {
            commands.AddRange(CommandHelper.TouchInStairs(lengths, zInternals));
        }

        public static void TouchInStairs(this List<BaseCommand> commands, List<int> lengths, List<int> zInternals, int repeatTimes)
        {
            commands.AddRange(CommandHelper.TouchInStairs(lengths, zInternals, repeatTimes));
        }

        public static void Rotate(this List<BaseCommand> commands, int steps)
        {
            commands.Add(CommandHelper.Rotate(steps));
        }

        public static void Reset(this List<BaseCommand> commands)
        {
            commands.Add(CommandHelper.Reset());
        }

        public static PoseCommand Reset()
        {
            return new PoseCommand(0, 0, 0);
        }

        public static GCommand Rotate(int steps)
        {
            return new GCommand(0, 0, steps);
        }

        public static PauseCommand WaitForTouch()
        {
            return new PauseCommand(WaitingTimeOutSeconds, RefreshIntervalMillSeconds);
        }

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

        [Obsolete("Use coordinate api")]
        public static GCommand ChangeLength(int dist)
        {
            return new GCommand(dist, -1 * dist, 0);
        }

        public static List<BaseCommand> TouchInSameRadius(List<int> zInternals)
        {
            return TouchInSameRadius(zInternals, 0);
        }

        public static List<BaseCommand> TouchInSameRadius(List<int> zInternals, int repeatTimes)
        {
            var result = new List<BaseCommand>();

            foreach (var z in zInternals)
            {
                result.Add(new GCommand(0, 0, z));

                result.AddRange(Tap());
                result.Add(new PauseCommand(WaitingTimeOutSeconds, RefreshIntervalMillSeconds));

                for (var i = 1; i <= repeatTimes; i++)
                {
                    result.AddRange(Tap());
                    result.Add(new PauseCommand(WaitingTimeOutSeconds, RefreshIntervalMillSeconds));
                }
            }

            return result;
        }

        public static List<BaseCommand> TouchInStairs(List<int> lengths, List<int> zInternals)
        {
            return TouchInStairs(lengths, zInternals, 0);

        }

        public static List<BaseCommand> TouchInStairs(List<int> lengths, List<int> zInternals, int repeatTimes)
        {
            if(lengths.Count != zInternals.Count)
            {
                throw new ArgumentException();
            }

            var result = new List<BaseCommand>();

            for(var j = 0; j < lengths.Count; j++)
            {
                result.Add(CommandHelper.Rotate(zInternals[j]));
                result.Add(CommandHelper.ChangeLength(lengths[j]));

                result.AddRange(Tap());
                result.Add(new PauseCommand(WaitingTimeOutSeconds, RefreshIntervalMillSeconds));

                for (var i = 1; i <= repeatTimes; i++)
                {
                    result.AddRange(Tap());
                    result.Add(new PauseCommand(WaitingTimeOutSeconds, RefreshIntervalMillSeconds));
                }
            }

            return result;
        }
    }
}
