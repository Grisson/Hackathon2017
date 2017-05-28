using ArmController.lib.Data;
using ArmController.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib
{
    public class PointsOnSameLine
    {
        public List<Tuple<PosePosition, TouchPoint>> Points = new List<Tuple<PosePosition, TouchPoint>>();
    }

    public static class Calibrator
    {
        private const double Tolerance = 0.01;

        public static List<Tuple<PosePosition, TouchPoint>> MapPoseAndTouch(SortedList<long, PosePosition> PosePositions, SortedList<long, TouchPoint> TouchPoints)
        {
            var PoseTouchMapping = new List<Tuple<PosePosition, TouchPoint>>();

            // Pose Position and TouchPoint here
            var poseTimestamp = PosePositions.Keys;
            var touchTimestamp = TouchPoints.Keys;

            var poseIndex = 0;
            var touchIndex = 0;

            for (; poseIndex < poseTimestamp.Count && touchIndex < touchTimestamp.Count;)
            {
                if (touchTimestamp[touchIndex] < poseTimestamp[poseIndex])
                {
                    touchIndex++;
                }
                else if ((poseTimestamp[poseIndex] < touchTimestamp[touchIndex]) &&
                    ((poseIndex + 1 >= poseTimestamp.Count) || (touchTimestamp[touchIndex] < poseTimestamp[poseIndex + 1])))
                {
                    var pose = PosePositions[poseTimestamp[poseIndex]];
                    pose.X += CommandHelper.LiftUpDistance;
                    pose.Y += CommandHelper.LiftUpDistance;
                         
                    PoseTouchMapping.Add(new Tuple<PosePosition, TouchPoint>(pose, TouchPoints[touchTimestamp[touchIndex]]));

                    poseIndex++;
                    touchIndex++;
                }
                else
                {
                    poseIndex++;
                }
            }

            return PoseTouchMapping;
        }

        public static List<Tuple<PosePosition, TouchPoint>> TouchPairsOnXAxis(List<Tuple<PosePosition, TouchPoint>> rawPoints)
        {
            var result = new List<Tuple<PosePosition, TouchPoint>>(); ;
            for (var i = 0; i < (rawPoints.Count - 1); i++)
            {
                if (rawPoints[i].Item1.Z == 0)
                {
                    result.Add(rawPoints[i]);
                }
            }

            return result;
        }

        // a + bx + cy = 0;
        public static TouchPoint[] TouchPointsOnXAxis(List<Tuple<PosePosition, TouchPoint>> rawPoints)
        {
            var result = new List<TouchPoint>(); ;
            for (var i = 0; i < (rawPoints.Count - 1); i++)
            {
                if (rawPoints[i].Item1.Z == 0)
                {
                    result.Add(rawPoints[i].Item2);
                }
            }

            return result.ToArray();
        }

        public static TouchPoint[][] TouchPointsOnSameRadius(List<Tuple<PosePosition, TouchPoint>> rawPoints)
        {
            var result = new List<TouchPoint[]>();

            var dict = new Dictionary<string, List<TouchPoint>>();
            for (var i = 0; i < (rawPoints.Count - 1); i++)
            {
                var x = rawPoints[i].Item1.X;
                var y = rawPoints[i].Item1.Y;

                var k = $"{x}|{y}";

                if (!dict.ContainsKey(k))
                {
                    dict[k] = new List<TouchPoint>();
                }

                dict[k].Add(rawPoints[i].Item2);
            }

            foreach (var k in dict.Keys)
            {
                if (dict[k].Count >= 3)
                {
                    result.Add(dict[k].ToArray());
                }
            }

            return result.ToArray();
        }

        // Length = F(distance)
        public static Tuple<double, double> MapLength(List<Tuple<PosePosition, TouchPoint>> rawPoints, TouchPoint centerPoint)
        {
            var X = new List<double>();
            var Y = new List<double>();

            foreach(var p in rawPoints)
            {
                var x = MathHelper.CalculateEuclideanDistance(new[] { p.Item2.X, p.Item2.Y }, new[] { centerPoint.X, centerPoint.Y }); 
                X.Add(x);
                var y = ArmPositionCalculator.SharedInstance.ToCoordinate(p.Item1).Item1;
                Y.Add(y);
            }

            var result = MathHelper.CalculateLine(X.ToArray(), Y.ToArray());

            return result;
        }

        // a + bx + cy = 0;
        public static Tuple<double, double> FindFx(TouchPoint[] points)
        {
            var X = new List<double>();
            var Y = new List<double>();

            foreach (var p in points)
            {
                var x = p.X;
                var y = p.Y;
                X.Add(x);
                Y.Add(y);
            }

            var result = MathHelper.CalculateLine(X.ToArray(), Y.ToArray());

            return result;
        }


        #region retired

        public static List<List<Tuple<PosePosition, TouchPoint>>> MapPointsOnSameLine(List<Tuple<PosePosition, TouchPoint>> rawPoints)
        {
            Dictionary<double, List<Tuple<PosePosition, TouchPoint>>> dict = new Dictionary<double, List<Tuple<PosePosition, TouchPoint>>>();
            List<List<Tuple<PosePosition, TouchPoint>>> result = new List<List<Tuple<PosePosition, TouchPoint>>>(); ;
            for (var i = 0; i < (rawPoints.Count - 1); i++)
            {
                for (var j = i + 1; j < rawPoints.Count; j++)
                {
                    var z1 = rawPoints[i].Item1.Z;
                    var z2 = rawPoints[j].Item1.Z;

                    // Compare the values
                    // The output to the console indicates that the two values are equal
                    if (z1 == z2)
                    {
                        result.Add(new List<Tuple<PosePosition, TouchPoint>> { rawPoints[i], rawPoints[j] });
                        break;
                    }
                }
            }

            return result;
        }

        public static List<Tuple<double, double, double>> CalculatorBisectorLines(List<Tuple<PosePosition, TouchPoint>> PoseTouchMapping)
        {
            var lines = new List<Tuple<double, double, double>>();

            for (var i = 0; i < PoseTouchMapping.Count - 1; i++)
            {
                for (var j = i + 1; j < PoseTouchMapping.Count; j++)
                {
                    var agentAPosition = PoseTouchMapping[i].Item1;
                    var agentBPosition = PoseTouchMapping[j].Item1;

                    if ((Math.Abs(agentAPosition.X - agentBPosition.X) < Tolerance) &&
                        (Math.Abs(agentAPosition.Y - agentBPosition.Y) < Tolerance) &&
                        (Math.Abs(agentAPosition.Z - agentBPosition.Z) > Tolerance))
                    {
                        // touch point should on same cycle

                        var touchPointA = PoseTouchMapping[i].Item2;
                        var touchPointB = PoseTouchMapping[j].Item2;

                        var line = MathHelper.CalculatorPerpendicularBisector(touchPointA, touchPointB);
                        lines.Add((line));
                    }
                }
            }

            return lines;
        }

        public static List<TouchPoint> CalculatorCenterPoints(List<Tuple<double, double, double>> lines)
        {
            var points = new List<TouchPoint>();

            for (var i = 0; i < lines.Count - 1; i++)
            {
                for (var j = i + 1; j < lines.Count; j++)
                {
                    var p = MathHelper.Intersect(lines[i], lines[j]);
                    points.Add(p);
                }
            }

            return points;
        }

        public static List<List<Tuple<PosePosition, TouchPoint>>> MapPointsOnSameAngle(List<Tuple<PosePosition, TouchPoint>> rawPoints)
        {
            Dictionary<double, List<Tuple<PosePosition, TouchPoint>>> dict = new Dictionary<double, List<Tuple<PosePosition, TouchPoint>>>();
            List<List<Tuple<PosePosition, TouchPoint>>> result = new List<List<Tuple<PosePosition, TouchPoint>>>(); ;
            for (var i = 0; i < (rawPoints.Count - 1); i++)
            {
                for (var j = i + 1; j < rawPoints.Count; j++)
                {
                    var z1 = rawPoints[i].Item1.Z;
                    var z2 = rawPoints[j].Item1.Z;

                    // Compare the values
                    // The output to the console indicates that the two values are equal
                    if (z1 == z2)
                    {
                        result.Add(new List<Tuple<PosePosition, TouchPoint>> { rawPoints[i], rawPoints[j] });
                        break;
                    }
                }
            }

            return result;
        }

        public static Tuple<double, double> CalculatorZ(List<List<Tuple<PosePosition, TouchPoint>>> pointsOnLine)
        {
            Tuple<double, double> result = null;
            if ((pointsOnLine == null) || (pointsOnLine.Count < 0))
            {
                return result;
            }

            var zAngleMappings = new List<Tuple<double, double>>();
            foreach (var l in pointsOnLine)
            {
                if (l.Count <= 1)
                {
                    continue;
                }

                var p1 = l[0];
                var p2 = l[1];

                var deltaX = Math.Abs(p1.Item2.X - p2.Item2.X);
                var deltaY = Math.Abs(p1.Item2.Y - p2.Item2.Y);

                var angle = Math.Atan2(deltaX, deltaY);

                if (double.IsNaN(angle))
                {
                    continue;
                }

                var factor = 1;

                var tmp = (p1.Item2.X - p2.Item2.X) * (p1.Item2.Y - p2.Item2.Y);
                if (tmp < 0)
                {
                    factor = -1;
                }

                zAngleMappings.Add(new Tuple<double, double>(p1.Item1.Z, angle * factor));
            }

            var listOfA = new List<double>();
            if (zAngleMappings.Count > 1)
            {
                for (var i = 0; i < (zAngleMappings.Count - 1); i++)
                {
                    for (var j = i + 1; j < zAngleMappings.Count; j++)
                    {
                        var deltaAngel = zAngleMappings[i].Item2 - zAngleMappings[j].Item2;
                        var deltaZ = zAngleMappings[i].Item1 - zAngleMappings[j].Item1;
                        if (deltaZ == 0)
                        {
                            continue;
                        }

                        var a = deltaAngel / deltaZ;
                        if (!double.IsNaN(a))
                        {
                            listOfA.Add(a);
                        }
                    }
                }
            }
            else
            {
                return result;
            }

            var averageA = listOfA.Average();

            var listOfB = new List<double>();
            foreach (var pair in zAngleMappings)
            {
                var b = pair.Item2 - (pair.Item1 * averageA);
                if (!double.IsNaN(b))
                {
                    listOfB.Add(b);
                }
            }
            var averageB = listOfB.Average();

            result = new Tuple<double, double>(averageA, averageB);

            return result;
        }
       
        #endregion
    }
}
