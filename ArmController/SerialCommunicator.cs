using System;
using System.IO.Ports;

namespace ArmController
{
    internal class SerialCommunicator
    {
        private int _baudRate;
        private bool _disposed;
        private string _portName;


        public SerialPort Port;

        public SerialCommunicator()
        {
        }

        public SerialCommunicator(string portName, int baudRate)
        {
            _portName = portName;
            _baudRate = baudRate;
        }

        public bool IsConnected => Port?.IsOpen ?? false;

        public void Connect()
        {
            if (Port == null)
            {
                Port = new SerialPort(_portName, _baudRate, Parity.None)
                {
                    StopBits = StopBits.One,
                    DataBits = 8
                };
            }

            if (!Port.IsOpen)
            {
                Port.Open();
            }
        }

        public void Close()
        {
            if ((Port != null) && Port.IsOpen)
            {
                Port.Close();
            }
        }

        public void SendData(byte[] data)
        {
            Port.Write(data, 0, data.Length);
        }

        public void WriteLine(string data)
        {
            Port.WriteLine(data);
        }

        public void Connect(string portName, int baudRate)
        {
            _baudRate = baudRate;
            _portName = portName;

            Connect();
        }

        public void StartRead(SerialDataReceivedEventHandler dataReceivedCallBack)
        {
            Port.DataReceived += dataReceivedCallBack;
        }

        #region Disposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            Close();

            _disposed = true;
        }

        ~SerialCommunicator()
        {
            Dispose(false);
        }

        #endregion
    }
}