using System;

namespace ArmController.lib.Data
{
    public class PosePosition
    {
        public double X;
        public double Y;
        public double Z;
        public long TimeStamp;

        public PosePosition()
        {
            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public PosePosition Clone()
        {
            return new PosePosition()
            {
                X = this.X,
                Y = this.Y,
                Z = this.Z,
            };
        }

        public PosePosition Incremental(double x, double y, double z)
        {
            return new PosePosition()
            {
                X = this.X += x,
                Y = this.Y += y,
                Z = this.Z += z,
            };
        }

        public static PosePosition InitializePosition()
        {
            return new PosePosition()
            {
                X = 0,
                Y = 0,
                Z = 0,
            };
        }
    }
}
