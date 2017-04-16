using System;
using ArmController.lib.Data;

namespace ArmController
{
    public class CommandHistory
    {
        public Guid CommandHistoryId;

        public PosePosition CurrentPosePosition;
        public DateTime ReceiveTimeStamp;
        public string Response;


        public DateTime SendTimeStamp;

        public double XDelta;
        public double YDelta;
        public double ZDelta;


        public CommandHistory(double xD, double yD, double zD, PosePosition position) : this()
        {
            XDelta = xD;
            YDelta = yD;
            ZDelta = zD;

            CurrentPosePosition = position;
            Command = $"G91 G0 X{XDelta} Y{YDelta} Z{ZDelta}";
        }

        public CommandHistory(string c)
        {
            Command = c;
        }

        public CommandHistory()
        {
            CommandHistoryId = Guid.NewGuid();
        }

        public string Command { get; }

        public PosePosition NextPosePosition 
            => CurrentPosePosition?.Incremental(XDelta, YDelta, ZDelta);

        public void Receive(string responseText)
        {
            Response = responseText;
            ReceiveTimeStamp = DateTime.Now;
        }

        public string ToSendLog()
        {
            return $"[{SendTimeStamp.ToLongTimeString()}]:{Command}";
        }

        public string ToReceiveLog()
        {
            var seconds = (ReceiveTimeStamp - SendTimeStamp).Milliseconds;
            return $"[{seconds}ms]:{Response}";
        }
    }
}