using ArmController.Models.Data;
using System;
using System.Runtime.Serialization;

namespace ArmController.Models.Command
{
    [DataContract]
    public class GCommand : BaseCommand
    {
        [DataMember]
        public int XDelta { get; set; }
        [DataMember]
        public int YDelta { get; set; }
        [DataMember]
        public int ZDelta { get; set; }
        [DataMember]
        public PosePosition CurrentPosePosition { get; set; }

        [IgnoreDataMember]
        public long SendTimeStamp { get; set; }

        [IgnoreDataMember]
        public long ReceiveTimeStamp { get; set; }
        [IgnoreDataMember]
        public string Response { get; set; }

        [DataMember]
        public string CommandText { get; set; }

        public PosePosition NextPosePosition
            => CurrentPosePosition?.Incremental(XDelta, YDelta, ZDelta);

        public GCommand() : base()
        {
            this.Type = CommandType.GCode;
        }

        public GCommand(int xD, int yD, int zD) : this()
        {
            XDelta = xD;
            YDelta = yD;
            ZDelta = zD;

            CommandText = $"G91 G0 X{XDelta} Y{YDelta} Z{ZDelta}";
        }

        public GCommand(int xD, int yD, int zD, PosePosition position) : this(xD, yD, zD)
        {
            CurrentPosePosition = position ?? PosePosition.InitializePosition();
        }

        public GCommand(string c) : this()
        {
            CommandText = c;
        }

        public void Receive(string responseText)
        {
            Response = responseText;
            ReceiveTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public override string ToSendLog
        {
            get
            {
                return $"[{SendTimeStamp}]:{CommandText}";
            }
        }

        public void RefreshCommandText()
        {
            CommandText = $"G91 G0 X{XDelta} Y{YDelta} Z{ZDelta}";
        }

        public string ToReceiveLog()
        {
            var seconds = (ReceiveTimeStamp - SendTimeStamp);
            return $"[{seconds}ms]:{Response}";
        }
    }
}
