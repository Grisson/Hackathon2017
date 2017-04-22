using System;

namespace ArmController.lib.Data
{
    public class PosePosition
    {
        public double X;
        public double Y;
        public double Z;
        public long TimeStamp;

        public PosePosition() : this(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, 0, 0)
        {

        }

        public PosePosition(double x, double y, double z)
            : this(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), x, y, z)
        {

        }

        public PosePosition(long timeStamp, double x, double y, double z)
        {
            TimeStamp = timeStamp;
            X = x;
            Y = y;
            Z = z;
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
            return new PosePosition(0, 0, 0);
        }
    }
}
