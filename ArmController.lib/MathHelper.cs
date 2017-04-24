using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using ArmController.lib.Data;

namespace ArmController.lib
{
    public static class MathHelper
    {
        public static Tuple<double, double, double> CalculatorPerpendicularBisector(TouchResponse pointA, TouchResponse pointB)
        {
            return CalculatorPerpendicularBisector(pointA.TouchPoint, pointB.TouchPoint);
        }

        public static Tuple<double, double, double> CalculatorPerpendicularBisector(Point pa, Point pb)
        {
            // ax + by = c
            var a = 2.0 * (pa.X - pb.X);
            var b = 2.0 * (pa.Y - pb.Y);

            var x1 = pa.X;
            var y1 = pa.Y;
            var x2 = pb.X;
            var y2 = pb.Y;

            var c = (y1 * y1 - y2 * y2 + x1 * x1 - x2 * x2);

            return new Tuple<double, double, double>(a, b, c);
        }

        public static Point Intersect(Tuple<double, double, double> line1, Tuple<double, double, double> line2)
        {
            // for Ax + By = C
            var A1 = line1.Item1;
            var B1 = line1.Item2;
            var C1 = line1.Item3;

            var A2 = line2.Item1;
            var B2 = line2.Item2;
            var C2 = line2.Item3;

            var delta = A1 * B2 - A2 * B1;
            var x = (B2 * C1 - B1 * C2) / delta;
            var y = (A1 * C2 - A2 * C1) / delta;

            return new Point()
            {
                X = x,
                Y = y,
            };
        }

        public static Point MeanPoint(List<Point> points)
        {
            var count = points.Count;

            if (count <= 0)
            {
                return null;
            }

            var allX = 0.0;
            var allY = 0.0;

            foreach (var p in points)
            {
                allX += p.X;
                allY += p.Y;
            }


            return new Point()
            {
                X = allX / (count * 1.0),
                Y = allY / (count * 1.0),
            };
        }
    }
}
