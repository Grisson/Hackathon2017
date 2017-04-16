using System;

namespace ArmController.lib.Data
{
    public class TouchResponse
    {
        public double TouchPointX;
        public double TouchPointY;
        public long TouchTimeStamp;

        public TouchResponse()
        {
            TouchTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public TouchResponse(double X, double Y):this()
        {
            TouchPointX = X;
            TouchPointY = Y;
        }
    }
}
