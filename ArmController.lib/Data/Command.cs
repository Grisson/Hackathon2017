using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib.Data
{
    public class Command
    {
        public Guid CommandHistoryId;

        public PosePosition CurrentPosePosition;
        public DateTime ReceiveTimeStamp;
        public string Response;


        public DateTime SendTimeStamp;

        public double XDelta;
        public double YDelta;
        public double ZDelta;


        public Command(double xD, double yD, double zD, PosePosition position) : this()
        {
            XDelta = xD;
            YDelta = yD;
            ZDelta = zD;

            CurrentPosePosition = position;
            CommandText = $"G91 G0 X{XDelta} Y{YDelta} Z{ZDelta}";
        }

        public Command(string c)
        {
            CommandText = c;
        }

        public Command()
        {
            CommandHistoryId = Guid.NewGuid();
        }

        public string CommandText { get; }

        public PosePosition NextPosePosition
            => CurrentPosePosition?.Incremental(XDelta, YDelta, ZDelta);

        public void Receive(string responseText)
        {
            Response = responseText;
            ReceiveTimeStamp = DateTime.Now;
        }

        public string ToSendLog()
        {
            return $"[{SendTimeStamp.ToLongTimeString()}]:{CommandText}";
        }

        public string ToReceiveLog()
        {
            var seconds = (ReceiveTimeStamp - SendTimeStamp).Milliseconds;
            return $"[{seconds}ms]:{Response}";
        }
    }
}
