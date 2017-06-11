using ArmController.Models.Command;
using Hamsa.Common;
using Hamsa.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamsa.UI.Code
{
    public class TestMachine : BaseExecutableCode
    {
        [Flags]
        public enum Status
        {
            Idle,
            WaitingCommand,
            WaitingTouch,
        }

        public Camera Eye;
        public ThreeDOFArm Arm;

        public Queue<BaseCommand> CommandList;
        public BaseCommand CurrentCommand;
        public Status CurrentStatus;

        public override void Setup()
        {
            CommandList = new Queue<BaseCommand>();
            CurrentStatus = Status.Idle;

            Eye = new Camera(0);

            Arm = new ThreeDOFArm("COM4", 115200);
            Arm.Connect();
        }

        public override void Loop()
        {
            if ((CurrentStatus & Status.Idle) == Status.Idle)
            {
                if(CommandList.Count > 0)
                {
                    CurrentCommand = CommandList.Dequeue();
                    lock(this.SyncRoot)
                    {
                        CurrentStatus = CurrentStatus | Status.WaitingCommand;
                    }

                }
            }
        }

        public override void Cleanup()
        {
            Eye.Dispose();
            Arm.Dispose();
        }
    }
}
