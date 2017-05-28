using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.Models.Data
{
    public class PosePosition
    {
        public int X;
        public int Y;
        public int Z;
        public long TimeStamp;

        public PosePosition() : this(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, 0, 0)
        {

        }

        public PosePosition(int x, int y, int z)
            : this(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), x, y, z)
        {

        }

        public PosePosition(long timeStamp, int x, int y, int z)
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

        public PosePosition Incremental(int x, int y, int z)
        {
            return new PosePosition()
            {
                X = this.X + x,
                Y = this.Y + y,
                Z = this.Z + z,
            };
        }

        public static PosePosition InitializePosition()
        {
            return new PosePosition(0, 0, 0);
        }
    }
}
