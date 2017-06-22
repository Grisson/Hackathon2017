using Hamsa.Common;
using Hamsa.Device;
using Hamsa.REST;
using Hamsa.UI.Code;
using Microsoft.Rest;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                    var currentCoordinate = arm.ConvertToCoordinate(currentPose);
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
        

        private void PlayCodeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(PlayCodeBtn.Content.ToString(), "Play", StringComparison.CurrentCultureIgnoreCase))
            {
                engine = new CodeEngine<TestMachine>();
                engine.Run();
                PlayCodeBtn.Content = "Stop";
            }
            else
            {
                if (engine != null)
                {
                    engine.Stop();
                    engine.Dispose();
                }
                PlayCodeBtn.Content = "Play";
            }
        }

        private void TestCodeBtn_Click(object sender, RoutedEventArgs e)
        {
            var Brain = new CloudBrain(new Uri("http://10.125.169.141:8182"), new BasicAuthenticationCredentials());

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
