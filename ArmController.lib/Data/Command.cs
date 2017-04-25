using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib.Data
{
    public enum CommandType
    {
        GCode = 0,
        Waiting,
        ScreenShot,
    }

    public class Command
    {
        public Guid CommandHistoryId;

        public double XDelta;
        public double YDelta;
        public double ZDelta;
        public PosePosition CurrentPosePosition;

        public long SendTimeStamp;

        public long ReceiveTimeStamp;
        public string Response;

        public string CommandText { get; }

        public PosePosition NextPosePosition
            => CurrentPosePosition?.Incremental(XDelta, YDelta, ZDelta);

        public Command()
        {
            CommandHistoryId = Guid.NewGuid();
        }

        public Command(double xD, double yD, double zD, PosePosition position) : this()
        {
            XDelta = xD;
            YDelta = yD;
            ZDelta = zD;

            CurrentPosePosition = position ?? PosePosition.InitializePosition();
            CommandText = $"G91 G0 X{XDelta} Y{YDelta} Z{ZDelta}";
        }

        public Command(string c) : this()
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

        public string ToReceiveLog()
        {
            var seconds = (ReceiveTimeStamp - SendTimeStamp);
            return $"[{seconds}ms]:{Response}";
        }
    }
}
