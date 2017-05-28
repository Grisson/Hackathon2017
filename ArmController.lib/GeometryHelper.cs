using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;
using System;
using System.Linq;

namespace ArmController.lib
{
    public static class GeometryHelper
    {
        public static Tuple<double, double> CalculateGrad(double ax, double ay, double az)
        {
            double rrot = Math.Sqrt((ax * ax) + (ay * ay));
            double rside = Math.Sqrt((rrot * rrot) + (az * az));

            double rot = Math.Asin(ax / rrot);
            double high = Math.Acos((rside * 0.5) / 120) * 2;

            double low = 0;

            if (az > 0)
            {
                low = Math.Asin(rrot / rside) + ((Math.PI - high) / 2.0) - (Math.PI / 2.0);

            }
            else
            {
                low = Math.PI - Math.Asin(rrot / rside) + ((Math.PI - high) / 2.0) - (Math.PI / 2.0);
            }

            high = high + low;

            return new Tuple<double, double>(high, low);


        }

        public static double[] CalculatePlane(double[][] x)
        {
            Vector<double> y = Vector<double>.Build.Dense(x.Length);

            double[] result = Fit.MultiDim(x, y.ToArray<double>(), true, DirectRegressionMethod.NormalEquations);


            return result;
            //Vector<double> p = MultipleRegression.NormalEquations(X, y);
            //MultipleRegression.QR or MultipleRegression.Svd

            //double[] p = Fit.Polynomial(xdata, ydata, 3); // polynomial of order 3

            // warning: preliminary api
            //var p = WeightedRegression.Local(X, y, t, radius, kernel);
        }
    }
}
