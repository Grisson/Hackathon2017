using System;

namespace ArmController.lib.Data
{
    public class TouchResponse
    {
        public Point TouchPoint { get; set; }
        public long TouchTimeStamp { get; set; }

        public double X
        {
            get
            {
                if (TouchPoint != null)
                {
                    return TouchPoint.X;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double Y
        {
            get
            {
                if (TouchPoint != null)
                {
                    return TouchPoint.Y;
                }
                else
                {
                    return 0;
                }
            }
        }

        public TouchResponse()
        {
            TouchTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public TouchResponse(double x, double y) : this()
        {
            TouchPoint = new Point()
            {
                X = x,
                Y = y,
            };
        }
    }
}
