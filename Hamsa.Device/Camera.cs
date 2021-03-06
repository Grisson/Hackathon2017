﻿using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Hamsa.Common;
using System;
using System.Drawing;
using System.Threading;

namespace Hamsa.Device
{
    public class Camera : BaseDeviceWithCircularBuffer<Capture, Bitmap>, IPull<Bitmap>, ISubscription<Bitmap>
    {
        public static readonly string SUB_RETRIEVEFRAME = "SUB_RETRIEVEFRAME";

        protected Action<Bitmap> handlers;


        public Camera(int cameraId) : base(1)
        {
            Device = new Capture(cameraId);
            Device.ImageGrabbed += ProcessFrame;
        }

        public override Bitmap GetLatestData()
        {
            if(Device != null)
            {
                var aFrame = Device.QueryFrame();
                return (Bitmap)aFrame.Bitmap.Clone();
            }

            return null;
        }

        public void Start()
        {
            if(Device != null)
            {
                Device.Start();
                //new Thread(() => { Device.Start(); }).Start();

                //Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            if (Device != null)
            {
                Device.Stop();
            }
        }

        public override void CleanUp()
        {
            if(Device != null)
            {
                Device.Stop();
                Device.Dispose();
            }
        }

        public void Subscript(string eventName, Action<Bitmap> callBack)
        {
            handlers += callBack;
        }

        protected void ProcessFrame(object sender, EventArgs arg)
        {
            var _frame = new Mat();
            if (Device.Retrieve(_frame, 0))
            {
                DataBuffer.Enqueue(_frame.Bitmap);

                handlers?.Invoke(_frame.Bitmap);
            }
        }
    }
}
