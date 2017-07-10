using ArmController.Models.Data;
using System;
using System.Runtime.Serialization;

namespace ArmController.Models.Command
{
    [DataContract]
    public class PoseCommand : BaseCommand
    {
        [DataMember]
        public PosePosition TargetPose { get; set; }

        [IgnoreDataMember]
        public PosePosition NextPosePosition => TargetPose;

        [IgnoreDataMember]
        public long SendTimeStamp { get; set; }

        [IgnoreDataMember]
        public long ReceiveTimeStamp { get; set; }

        [IgnoreDataMember]
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
                return $"[{SendTimeStamp}]:{CommandText}";
            }
        }
    }
}
