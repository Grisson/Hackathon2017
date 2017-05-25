﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using ArmController.lib.Data;
using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;

namespace ArmController.lib
{
    public static class ExtensionMethod
    {
        public static double RandWithFiveDigites(this double a)
        {
            return Math.Round(a, 5, MidpointRounding.AwayFromZero);
        }

        public static bool IsEqualWithInTolerance(this double a, double b, double tolerance)
        {
            if (Math.Abs(a - b) < tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
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
                if(double.IsNaN(p.X) || double.IsNaN(p.Y))
                {
                    count--;
                    continue;
                }
                allX += p.X;
                allY += p.Y;
            }


            return new Point()
            {
                X = allX / (count * 1.0),
                Y = allY / (count * 1.0),
            };
        }

        public static double[] CalculateCentorOfCircle(TouchResponse[][] touchPoints)
        {
            List<double[]> X = new List<double[]>();
            List<double> Y = new List<double>();
            foreach(var pointsInSameRow in touchPoints)
            {
                var point0 = pointsInSameRow[0];
                var point1 = pointsInSameRow[1];
                var point2 = pointsInSameRow[2];

                var x0 = point0.TouchPoint.X;
                var y0 = point0.TouchPoint.Y;
                var x1 = point1.TouchPoint.X;
                var y1 = point1.TouchPoint.Y;
                var x2 = point2.TouchPoint.X;
                var y2 = point2.TouchPoint.Y;

                var Y0 = x1 * x1 + y1 * y1 - x0 * x0 - y0 * y0;
                var Y1 = x2 * x2 + y2 * y2 - x1 * x1 - y1 * y1;
                Y.Add(Y0);
                Y.Add(Y1);

                var X11 = (x1 - x0) * 2;
                var X12 = (y1 - y0) * 2;
                X.Add(new[] { X11, X12 });

                var X21 = (x2 - x1) * 2;
                var X22 = (y2 - y1) * 2;
                X.Add(new[] { X21, X22 });
            }

            double[] result = Fit.MultiDim(X.ToArray(), Y.ToArray(), false, DirectRegressionMethod.NormalEquations);

            return result;
        }

        public static double CalculateDistance(double[] center, double[] touchPoint)
        {
            return Distance.Euclidean(center, touchPoint);
        }

        public static Tuple<double, double> CalculateLine(double[] X, double[] Y)
        {
            var result = Fit.Line(X, Y);
            return result;
        }
    }
}
