using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamsa.Common
{
    public abstract class UsbDevice : BaseDevice<SerialPort>, ISubscription<string>, IPush<string>, IPush<byte[]>
    {
        private int _baudRate;
        private string _portName;

        private Action<string> DataReceivedHandler;

        public UsbDevice(string portName, int baudRate)
        {
            _portName = portName;
            _baudRate = baudRate;
        }

        public override void CleanUp()
        {
            Close();
        }

        public void Connect()
        {
            if (Device == null)
            {
                Device = new SerialPort(_portName, _baudRate, Parity.None)
                {
                    StopBits = StopBits.One,
                    DataBits = 8
                };

                Device.DataReceived += DataReceived;
            }

            if (!Device.IsOpen)
            {
                Device.Open();
            }
        }

        public void Close()
        {
            if ((Device != null) && Device.IsOpen)
            {
                Device.Close();
            }
        }

        protected abstract void DataReceived(object sender, SerialDataReceivedEventArgs e);
        //protected void DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    var port = sender as SerialPort;
        //    while (port.BytesToRead > 0)
        //    {
        //        var d = port.ReadLine();

        //        DataReceivedHandler?.Invoke(d);
        //    }
        //}

        void IPush<string>.Push(string data)
        {
            Device.WriteLine(data);
        }

        void IPush<byte[]>.Push(byte[] data)
        {
            Device.Write(data, 0, data.Length);
        }

        public void Subscript(string eventName, Action<string> callBack)
        {
            DataReceivedHandler += callBack;
        }
    }
}
