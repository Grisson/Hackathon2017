using ArmController.lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController.lib
{
    public class PointsOnSameLine
    {
        public List<Tuple<PosePosition, TouchResponse>> Points = new List<Tuple<PosePosition, TouchResponse>>();
    }

    public static class Calibrator
    {
        private const double Tolerance = 0.00001;

        public static List<Tuple<PosePosition, TouchResponse>> MapPoseAndTouch(SortedList<long, PosePosition> PosePositions, SortedList<long, TouchResponse> TouchPoints)
        {
            var PoseTouchMapping = new List<Tuple<PosePosition, TouchResponse>>();

            // Pose Position and TouchResponse here
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

                    PoseTouchMapping.Add(new Tuple<PosePosition, TouchResponse>(PosePositions[poseTimestamp[poseIndex]], TouchPoints[touchTimestamp[touchIndex]]));

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


        public static List<Tuple<double, double, double>> CalculatorBisectorLines(List<Tuple<PosePosition, TouchResponse>> PoseTouchMapping)
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

        public static List<Point> CalculatorCenterPoints(List<Tuple<double, double, double>> lines)
        {
            var points = new List<Point>();

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


        public static List<List<Tuple<PosePosition, TouchResponse>>> MapPointsOnSameLine(List<Tuple<PosePosition, TouchResponse>> rawPoints)
        {
            Dictionary<double, List<Tuple<PosePosition, TouchResponse>>> dict = new Dictionary<double, List<Tuple<PosePosition, TouchResponse>>>();

            foreach(var p in rawPoints)
            {
                var pos = p.Item1;
                if(!dict.ContainsKey(pos.Z))
                {
                    dict[pos.Z] = new List<Tuple<PosePosition, TouchResponse>>();
                }
                dict[pos.Z].Add(p);
            }

            List<List<Tuple<PosePosition, TouchResponse>>> result = null;
            foreach(var k in dict.Keys)
            {
                if(dict[k].Count > 1)
                {
                    if(result == null)
                    {
                        result = new List<List<Tuple<PosePosition, TouchResponse>>>();
                    }
                    result.Add(dict[k]);
                }
            }

            return result;
        }


        public static Tuple<int, int> CalculatorZ(List<List<Tuple<PosePosition, TouchResponse>>> pointsOnLine)
        {
            Tuple<int, int> result = null;
            if ((pointsOnLine == null) || (pointsOnLine.Count<0))
            {
                return result;
            }

            foreach(var l in pointsOnLine)
            {
                if(l.Count <= 1)
                {
                    continue;
                }

                var topLeftP = l[0];
                var bottomRightP = l[1];

                if(bottomRightP.Item2.TouchPoint.X < topLeftP.Item2.TouchPoint.X)
                {
                    topLeftP = l[1];
                    bottomRightP = l[0];
                }

            }

            return result;
        }
    }
}
