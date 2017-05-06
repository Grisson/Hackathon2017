using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ArmController.lib.Data;
using System.Threading;

namespace ArmController
{
    public partial class MainWindow
    {
        #region UI Events

        private void ComboBoxPort_OnDropDownOpened(object sender, EventArgs e)
        {
            ComboBoxPort.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = SerialPort.GetPortNames() });
        }

        private void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            if ((_serialPort != null) && _serialPort.IsConnected)
            {
                _serialPort.Dispose();
                _serialPort = null;

                _dataContext.AddOutput($"Disconnected");
                Scroller.ScrollToBottom();
                ConnectButton.Content = "Conntect";

                _testBrain.UnRegisterTestAgent();

                Title = Title.Substring(0, Title.Length - Title.LastIndexOf("-", StringComparison.Ordinal));
            }
            else
            {
                var portName = ComboBoxPort.SelectedValue.ToString();
                var baud = int.Parse(ComboBoxBaud.SelectedValue.ToString());
                _serialPort = new SerialCommunicator(portName, baud);
                _serialPort.Connect();
                _serialPort.StartRead(DataReceivedHandler);

                if (_serialPort.IsConnected)
                {
                    _dataContext.AddOutput($"Connected to {portName}");
                    Scroller.ScrollToBottom();
                    ConnectButton.Content = "Disconntect";
                    Title = Title + " - Connected";

                    _testBrain.RegisterTestAgent(_deviceId.ToString());
                }
            }
        }

        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_serialPort.IsConnected && (_currentCommand == null))
            {
                var command = "$";
                _currentCommand = new Command(command);
                _serialPort.Port.WriteLine(command);
                _dataContext.AddOutput(_currentCommand.ToSendLog());
            }
        }

        private void SendCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_serialPort.IsConnected)
            {
                return;
            }

            var xInc = TextToDouble(XCommandTextBox.Text);
            var yInc = TextToDouble(YCommandTextBox.Text);
            var zInc = TextToDouble(ZCommandTextBox.Text);
            var newCommand = new Command(xInc, yInc, zInc, _currentPosePosition);

            _commands.Enqueue(newCommand);

            new Thread(ExcuteCommand).Start();
        }

        private void SendTouchButton_OnClick(object sender, RoutedEventArgs e)
        {
            var x = TextToDouble(TouchXTextBox.Text);
            var y = TextToDouble(TouchYTextBox.Text);
            _testBrain.ReportTouchBegin(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), x, y);
        }

        private void switchCameraBtn_Click(object sender, RoutedEventArgs e)
        {
            if (shouldDetectCamera)
            {
                if((currentCameraId == maxCameraId) && (currentCameraId >= 2 ))
                {
                    currentCameraId = 0;
                    shouldDetectCamera = false;
                }

                try
                {
                    InitCamera(++currentCameraId);
                    maxCameraId = currentCameraId;
                }
                catch
                {
                    // set currentCameraId to 0 or -1
                    currentCameraId = Math.Min(0, maxCameraId);
                    _camera = null;

                }
            }
            else
            {
                if (currentCameraId < 0)
                {
                    // no camera
                    return;
                }

                currentCameraId++;
                if (currentCameraId > maxCameraId)
                {
                    currentCameraId = 0;
                }

                InitCamera(currentCameraId);
            }

            if ((_camera != null) && ( 0 <= currentCameraId))
            {
                switchCameraBtn.Content = $"Switch(current:{currentCameraId})";
            }
            else
            {
                switchCameraBtn.Content = "Switch";
            }
        }

        private void startCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_camera == null)
            {
                return;
            }

            if (string.Equals(startCaptureBtn.Content.ToString(), "start", StringComparison.CurrentCultureIgnoreCase))
            {
                startCaptureBtn.Content = "stop";
                _camera.Start();
            }
            else
            {
                startCaptureBtn.Content = "start";
                _camera.Stop();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LiveVideoBox.Source = null;
                });
            }

        }

        private void flipCaptureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_camera != null)
            {
                _camera.FlipHorizontal = !_camera.FlipHorizontal;
            }

        }
        #endregion
    }
}
