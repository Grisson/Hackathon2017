using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib.Data
{
    public class GCommand : BaseCommand
    {
        public bool ResetPosition { get; set; }

        public double XDelta { get; set; }
        public double YDelta { get; set; }
        public double ZDelta { get; set; }
        public PosePosition CurrentPosePosition { get; set; }

        public long SendTimeStamp { get; set; }

        public long ReceiveTimeStamp { get; set; }
        public string Response { get; set; }

        public string CommandText { get; set; }

        public PosePosition NextPosePosition
            => CurrentPosePosition?.Incremental(XDelta, YDelta, ZDelta);

        public GCommand() : base()
        {
            
        }

        public GCommand(double xD, double yD, double zD) : this()
        {
            XDelta = xD;
            YDelta = yD;
            ZDelta = zD;

            CommandText = $"G91 G0 X{XDelta} Y{YDelta} Z{ZDelta}";
        }

        public GCommand(double xD, double yD, double zD, PosePosition position) : this(xD, yD, zD)
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

        public string ToSendLog()
        {
            return $"[{SendTimeStamp}]:{CommandText}";
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
