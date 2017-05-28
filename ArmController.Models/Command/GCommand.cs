﻿using ArmController.Models.Data;
using System;

namespace ArmController.Models.Command
{
    public class GCommand : BaseCommand
    {
        public int XDelta { get; set; }
        public int YDelta { get; set; }
        public int ZDelta { get; set; }
        public PosePosition CurrentPosePosition { get; set; }

        public long SendTimeStamp { get; set; }

        public long ReceiveTimeStamp { get; set; }
        public string Response { get; set; }

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
