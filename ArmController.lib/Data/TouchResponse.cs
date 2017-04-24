using System;

namespace ArmController.lib.Data
{
    public class TouchResponse
    {
        public Point TouchPoint { get; set; }
        public long TouchTimeStamp { get; set; }

        public TouchResponse()
        {
            TouchTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public TouchResponse(double x, double y):this()
        {
            TouchPoint = new Point()
            {
                X = x,
                Y = y,
            };
        }
    }
}
