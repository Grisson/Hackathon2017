﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ArmController
{
    public partial class MainWindow
    {
        private Capture _camera;

        private int maxCameraId = -1;
        private int currentCameraId = -1;
        private bool shouldDetectCamera = true;
        private Mat _frame;

        protected void InitCamera(int cameraId)
        {
            try
            {
                _camera = new Capture(cameraId);

                Mat tmp;
                tmp = _camera.QuerySmallFrame();
                _camera.Retrieve(tmp, 0);

                _camera.ImageGrabbed += ProcessFrame;
                _frame = new Mat();
            }
            catch (Exception ex)
            {
                _camera?.Dispose();
                shouldDetectCamera = false;

                throw;
            }
        }


        private void ProcessFrame(object sender, EventArgs arg)
        {
            if (_camera != null && _camera.Ptr != IntPtr.Zero)
            {
                _camera.Retrieve(_frame, 0);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LiveVideoBox.Source = BitmapSourceConvert.ToBitmapSource(_frame);
                });
                
                //captureImageBox.Image = _frame;
                //grayscaleImageBox.Image = _grayFrame;
                //smoothedGrayscaleImageBox.Image = _smoothedGrayFrame;
                //cannyImageBox.Image = _cannyFrame;
            }
        }

        /*
         Mat image = new Mat(100, 400, DepthType.Cv8U, 3);
         image.SetTo(new Bgr(255, 255, 255).MCvScalar);
         CvInvoke.PutText(image, "Hello, world", new System.Drawing.Point(10, 50), Emgu.CV.CvEnum.FontFace.HersheyPlain, 3.0, new Bgr(255.0, 0.0, 0.0).MCvScalar);

         image1.Source = BitmapSourceConvert.ToBitmapSource(image); 
         */


    }
}