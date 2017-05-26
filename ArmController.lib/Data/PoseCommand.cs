using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib.Data
{
    public class PoseCommand : BaseCommand
    {
        public PosePosition TargetPose { get; set; }

        public PosePosition NextPosePosition => TargetPose;

        public long SendTimeStamp { get; set; }

        public long ReceiveTimeStamp { get; set; }

        public string Response { get; set; }

        public PoseCommand() : base()
        {
            this.Type = CommandType.Pose;
        }

        public PoseCommand(int x, int y, int z) : this()
        {
            TargetPose = new PosePosition(x, y, z);
        }

        public string CommandText
        {
            get
            {
                return $"G90 G0 X{TargetPose.X} Y{TargetPose.Y} Z{TargetPose.Z}";
            }
        }

        public void Receive(string responseText)
        {
            Response = responseText;
            ReceiveTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public string ToReceiveLog()
        {
            var seconds = (ReceiveTimeStamp - SendTimeStamp);
            return $"[{seconds}ms]:{Response}";
        }

        public override string ToSendLog
        {
            get
            {
                return $"[{SendTimeStamp}]:GoTo:{TargetPose.X}/{TargetPose.Y}/{TargetPose.Z}";
            }
        }
    }
}
