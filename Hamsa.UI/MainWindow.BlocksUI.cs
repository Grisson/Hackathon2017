using Hamsa.Common;
using Hamsa.UI.Code;
using System;
using System.Windows;
using System.Windows.Media;

namespace Hamsa.UI
{
    public partial class MainWindow
    {

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(PlayBtn.Content.ToString(), "Play", StringComparison.CurrentCultureIgnoreCase))
            {
                engine = new CodeEngine<TrackFaceSample>();
                engine.Run();
                PlayBtn.Content = "Stop";
            }
            else
            {
                if (engine != null)
                {
                    engine.Stop();
                    engine.Dispose();
                }
                PlayBtn.Content = "Play";
            }

        }

        private void GenerateBlocks()
        {
            var x0 = 50;
            var y0 = 50;
            var SetUpBlock = DrawRectangleCodeBlock(new System.Windows.Point(x0, y0), 50, 33, true, true, false, false);
            var InitCamera = DrawRectangleCodeBlock(new System.Windows.Point(50, 50 + 33), 100, 42, true, true, false, true);
            var InitCameraId = DrawRectangleCodeBlock(new System.Windows.Point(50 + 100, 50 + 33), 40, 42, false, false, true, false);
            var StartCamera = DrawRectangleCodeBlock(new System.Windows.Point(50, 50 + 33 + 42), 100, 42, true, false, false, true);

            var initArm = DrawRectangleCodeBlock(new System.Windows.Point(50, 50 + 33 + 42 * 2), 100, 42, true, true, false, true);
            var initArmCom = DrawRectangleCodeBlock(new System.Windows.Point(50 + 100, 50 + 33 + 42 * 2), 40, 42, false, false, true, false);

            var initBrian = DrawRectangleCodeBlock(new System.Windows.Point(50, 50 + 33 + 42 * 3), 100, 42, true, false, false, false);

            var loop = DrawLoopCodeBlock(new System.Windows.Point(50, 50 + 33 + 42 * 4 + 10), 100 + LoopBlockLeftThicknes, 50 + 42 * 5, false, false, false);
            var xInner = 50 + LoopBlockLeftThicknes;
            var yInner = 50 + 33 + 42 * 4 + 10 + LoopBlockTopThicknes;
            var takePhoto = DrawRectangleCodeBlock(new System.Windows.Point(xInner, yInner), 100, 42, true, true, false, false);
            var detectFace = DrawRectangleCodeBlock(new System.Windows.Point(xInner, yInner + 42), 100, 42, true, true, false, false);
            var mapLocation = DrawRectangleCodeBlock(new System.Windows.Point(xInner, yInner + 42 * 2), 100, 42, true, true, false, false);
            var moveArm = DrawRectangleCodeBlock(new System.Windows.Point(xInner, yInner + 42 * 3), 100, 42, true, false, false, false);

            PathGeometry blocks = new PathGeometry();
            blocks.Figures.Add(SetUpBlock);
            blocks.Figures.Add(InitCamera);
            blocks.Figures.Add(InitCameraId);
            blocks.Figures.Add(StartCamera);
            blocks.Figures.Add(initArm);
            blocks.Figures.Add(initArmCom);
            blocks.Figures.Add(initBrian);

            blocks.Figures.Add(loop);
            blocks.Figures.Add(takePhoto);
            blocks.Figures.Add(detectFace);
            blocks.Figures.Add(mapLocation);
            blocks.Figures.Add(moveArm);

            CodeBlocks.Stroke = System.Windows.Media.Brushes.Black;
            CodeBlocks.StrokeThickness = 2;
            CodeBlocks.Data = blocks;
        }

        private PathFigure DrawRectangleCodeBlock(System.Windows.Point startPoint, int width, int height, bool takeInput, bool hasOutput, bool isParameter, bool withParameter)
        {
            var block = new PathFigure();
            block.StartPoint = new System.Windows.Point(startPoint.X, startPoint.Y);
            // left
            if (isParameter)
            {
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y + height / 3), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X - 10, startPoint.Y + height / 3 - 5), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X - 10, startPoint.Y + 2 * height / 3 + 5), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y + 2 * height / 3), true));
            }
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y + height), true));


            if (hasOutput)
            {
                // bottom_
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 5, startPoint.Y + height), true));

                // bottom_
                //        \
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 5 + 2.5, startPoint.Y + height + 5), true));
                // bottom_
                //        \/
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 5 + 5, startPoint.Y + height), true));
                // bottom_  _
                //        \/
            }
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + height), true));

            // right 
            if (withParameter)
            {
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + 2 * height / 3), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width - 10, startPoint.Y + 2 * height / 3 + 5), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width - 10, startPoint.Y + height / 3 - 5), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + height / 3), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y), true));
            }
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y), true));

            if (takeInput)
            {
                // Top    _
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 10, startPoint.Y), true));
                // Top     _
                //        /
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 10 - 2.5, startPoint.Y + 5), true));
                // Top     _
                //       \/
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 10 - 5, startPoint.Y), true));
            }
            // Top _  _
            //      \/
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y), true));

            return block;
        }

        private PathFigure DrawLoopCodeBlock(System.Windows.Point startPoint, int width, int height, bool takeInput, bool hasOutput, bool withParameter)
        {

            var block = new PathFigure();
            block.StartPoint = new System.Windows.Point(startPoint.X, startPoint.Y);

            // |
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y + height), true));

            // |
            // |__
            if (hasOutput)
            {
                // bottom_
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 5, startPoint.Y + height), true));

                // bottom_
                //        \
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 5 + 2.5, startPoint.Y + height + 5), true));
                // bottom_
                //        \/
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 5 + 5, startPoint.Y + height), true));
                // bottom_  _
                //        \/
            }
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + height), true));

            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + height - LoopBlockBottomThicknes), true));

            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + LoopBlockLeftThicknes, startPoint.Y + height - LoopBlockBottomThicknes), true));

            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + LoopBlockLeftThicknes, startPoint.Y + LoopBlockTopThicknes), true));

            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + LoopBlockTopThicknes), true));

            // right 
            if (withParameter)
            {
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + 2 * LoopBlockTopThicknes / 3), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width - 10, startPoint.Y + 2 * LoopBlockTopThicknes / 3 + 5), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width - 10, startPoint.Y + LoopBlockTopThicknes / 3 - 5), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + LoopBlockTopThicknes / 3), true));
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y), true));
            }
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y), true));

            if (takeInput)
            {
                // Top    _
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 10, startPoint.Y), true));
                // Top     _
                //        /
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 10 - 2.5, startPoint.Y + 5), true));
                // Top     _
                //       \/
                block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + 10 - 5, startPoint.Y), true));
            }
            // Top _  _
            //      \/
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y), true));

            return block;
        }
    }
}
