using Emgu.CV;
using Emgu.CV.CvEnum;
using Hamsa.Common;
using Hamsa.Device;
using Hamsa.UI.Code;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hamsa.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Camera eye { get; set; }
        public ThreeDOFArm arm { get; set; }

        public ICodeEngine engine;

        public int LoopBlockTopThicknes = 42;
        public int LoopBlockLeftThicknes = 15;
        public int LoopBlockBottomThicknes = 20;

        public MainWindow()
        {
            InitializeComponent();

            GenerateBlocks();
        }

        private void CameraShowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(CameraShowBtn.Content.ToString(), "Show", StringComparison.CurrentCultureIgnoreCase))
            {
                var cameraId = int.Parse(CameraId.Text);
                eye = new Camera(cameraId);
                eye.Subscript("newFrame", ProcessFrame);
                eye.Start();

                CameraShowBtn.Content = "Hide";
            }
            else
            {
                if (eye != null)
                {
                    eye.Stop();
                    eye.Dispose();
                    LiveVideoBox.Source = null;
                }

                CameraShowBtn.Content = "Show";
            }

        }

        protected void ProcessFrame(Bitmap frame)
        {
            if ((Application.Current != null) && (Application.Current.Dispatcher != null))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LiveVideoBox.Source = BitmapSourceConvert.ToBitmapSource(frame);
                });
            }
        }

        private void ComboBoxPort_DropDownOpened(object sender, EventArgs e)
        {
            ComboBoxPort.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = SerialPort.GetPortNames() });
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(ConnectButton.Content.ToString(), "Disconntect", StringComparison.CurrentCultureIgnoreCase))
            {
                if (((arm != null) && arm.IsConnected))
                {
                    arm.Dispose();
                    arm = null;
                    ConnectButton.Content = "Conntect";

                    CurrentCoordinateX.Text = "";
                    CurrentCoordinateY.Text = "";
                    CurrentCoordinateZ.Text = "";
                }
            }
            else
            {
                if (arm == null)
                {
                    var portName = ComboBoxPort.SelectedValue.ToString();
                    var baud = 115200;
                    arm = new ThreeDOFArm(portName, baud);
                    arm.Subscript(string.Empty, handleArmCallback);
                }

                if (!arm.IsConnected)
                {
                    arm.Connect();
                }

                if (arm.IsConnected)
                {
                    ConnectButton.Content = "Disconntect";
                    var currentPose = arm.GetLatestData();
                    var currentCoordinate = arm.ToCoordinate(currentPose);
                    ShowPosition(currentCoordinate.Item1, currentCoordinate.Item2, currentCoordinate.Item3);
                    //var currentPosition =
                }
                //_serialPort.StartRead(DataReceivedHandler);
            }
        }

        protected void handleArmCallback(string data)
        {

        }

        protected void ShowPosition(double x, double y, double z)
        {
            CurrentCoordinateX.Text = $"X: {Math.Round(x, 2, MidpointRounding.AwayFromZero)}";
            CurrentCoordinateY.Text = $"Y: {Math.Round(y, 2, MidpointRounding.AwayFromZero)}";
            CurrentCoordinateZ.Text = $"Z: {Math.Round(z, 2, MidpointRounding.AwayFromZero)}";
        }


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

            var initArm= DrawRectangleCodeBlock(new System.Windows.Point(50, 50 + 33 + 42 * 2), 100, 42, true, true, false, true);
            var initArmCom= DrawRectangleCodeBlock(new System.Windows.Point(50 + 100, 50 + 33 + 42 * 2), 40, 42, false, false, true, false);

            var initBrian = DrawRectangleCodeBlock(new System.Windows.Point(50, 50 + 33 + 42 * 3), 100, 42, true, false, false, false);

            var loop = DrawLoopCodeBlock(new System.Windows.Point(50, 50 + 33 + 42 * 4 + 10), 100 + LoopBlockLeftThicknes, 50 + 42*5, false, false, false);
            var xInner = 50 + LoopBlockLeftThicknes;
            var yInner = 50 + 33 + 42 * 4 + 10 + LoopBlockTopThicknes;
            var takePhoto = DrawRectangleCodeBlock(new System.Windows.Point(xInner, yInner), 100, 42, true, true, false, false);
            var detectFace = DrawRectangleCodeBlock(new System.Windows.Point(xInner, yInner + 42), 100, 42, true, true, false, false);
            var mapLocation = DrawRectangleCodeBlock(new System.Windows.Point(xInner, yInner + 42*2), 100, 42, true, true, false, false);
            var moveArm = DrawRectangleCodeBlock(new System.Windows.Point(xInner, yInner + 42*3), 100, 42, true, false, false, false);

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
            if(isParameter)
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

            if(takeInput)
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

    public static class BitmapSourceConvert
    {
        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The poniter to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                return ToBitmapSource(source);
            }
        }

        public static BitmapSource ToBitmapSource(Bitmap source)
        {
            if (source == null)
            {
                return null;
            }

            IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ptr,
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(ptr); //release the HBitmap
            return bs;
        }

        public static Mat ToMat(BitmapSource source)
        {

            if (source.Format == PixelFormats.Bgra32)
            {
                Mat result = new Mat();
                result.Create(source.PixelHeight, source.PixelWidth, DepthType.Cv8U, 4);
                source.CopyPixels(Int32Rect.Empty, result.DataPointer, result.Step * result.Rows, result.Step);
                return result;
            }
            else if (source.Format == PixelFormats.Bgr24)
            {
                Mat result = new Mat();
                result.Create(source.PixelHeight, source.PixelWidth, DepthType.Cv8U, 3);
                source.CopyPixels(Int32Rect.Empty, result.DataPointer, result.Step * result.Rows, result.Step);
                return result;
            }
            else
            {
                throw new Exception(String.Format("Convertion from BitmapSource of format {0} is not supported.", source.Format));
            }
        }
    }
}
/*
 * FormattedText text = new FormattedText("Text to display",
        CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
        new Typeface("Tahoma"),
        16,
        Brushes.Black);
    Geometry geometry = text.BuildGeometry(new Point(5, 5));
 * private PathFigure DrawLeftTopLine(System.Windows.Point startPoint, int width, int height)
        {
            var block = new PathFigure();
            block.StartPoint = new System.Windows.Point(startPoint.X, startPoint.Y);
            // left
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y + height), true));
            // top
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y), true));

            return block;
        }

 *  private PathFigure DrawRectangle(System.Windows.Point startPoint, int width, int height, string text = "")
        {
            var block = new PathFigure();
            block.StartPoint = new System.Windows.Point(startPoint.X, startPoint.Y);
            // left
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y + height), true));
            // bottom
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y + height), true));
            // right 
            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X + width, startPoint.Y), true));

            block.Segments.Add(new LineSegment(new System.Windows.Point(startPoint.X, startPoint.Y), true));

            return block;
        }

 * 
 * 
 */
