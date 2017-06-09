﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Hamsa.Azure;
using Hamsa.Device;
using Microsoft.ProjectOxford.Face;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        public MainWindow()
        {
            InitializeComponent();
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
                if(eye != null)
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
                
                //_dataContext.AddOutput($"Disconnected");
                //Scroller.ScrollToBottom();
                //_currentPosePosition = PosePosition.InitializePosition();
                //ShowCurrentPosition();
                //_testBrain.UnRegisterTestAgent();

                //Title = Title.Substring(0, Title.Length - Title.LastIndexOf("-", StringComparison.Ordinal));
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

                if(!arm.IsConnected)
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


        private async void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (eye == null)
            {
                var cameraId = int.Parse(CameraId.Text);
                eye = new Camera(cameraId);
                eye.Subscript("newFrame", ProcessFrame);
                eye.Start();

                CameraShowBtn.Content = "Hide";
            }

            var img = eye.GetLatestData();
            img.Save("fromcamer.jpg");
           
            var cog = new Cognitive();

            var faces = await cog.DetectFaces(img);

            // map the location

            // Command ARM

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
