using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ArmController.lib.Data;
using System.Threading;
using ArmController.Executor;
using ArmController.lib;
using ArmController.Models.Command;
using ArmController.Models.Data;

namespace ArmController
{
    public partial class MainWindow
    {
        #region UI Events

        private void goToCoordinateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_serialPort.IsConnected)
            {
                return;
            }

            var coorX = TextToDouble(CoordinateXTextBox.Text);
            var coorY = TextToDouble(CoordinateYTextBox.Text);
            var coorZ = TextToDouble(CoordinateZTextBox.Text);
            var targetPose = _testBrain.ConvertCoordinatToPosition(new Tuple<double, double, double>(coorX, coorY, coorZ));
            var newCommand = new GCommand(targetPose.X - _currentPosePosition.X, 
                targetPose.Y - _currentPosePosition.Y, 
                targetPose.Z - _currentPosePosition.Z, 
                _currentPosePosition);

            _commands.Enqueue(newCommand);

            new Thread(CommandExecutor.SharedInstance.Execute).Start();
        }

        private void CalibButton_Click(object sender, RoutedEventArgs e)
        {
            this.GetProbCommandsFromServer();
        }

        private void TapButton_Click(object sender, RoutedEventArgs e)
        {
            var deserializedList = CommandHelper.Tap();

            foreach (var c in deserializedList)
            {
                _commands.Enqueue(c);
            }

            new Thread(CommandExecutor.SharedInstance.Execute).Start();

        }

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
                _currentPosePosition = PosePosition.InitializePosition();
                ShowCurrentPosition();
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
                CommandStore.SharedInstance.Enqueue(new GCommand(command));
                //_currentCommand = new GCommand(command);
                //_serialPort.Port.WriteLine(command);
                //_dataContext.AddOutput(((GCommand)_currentCommand).ToSendLog());
                new Thread(CommandExecutor.SharedInstance.Execute).Start();
            }
        }

        private void SendCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_serialPort.IsConnected)
            {
                return;
            }

            var xInc = TextToInt(XCommandTextBox.Text);
            var yInc = TextToInt(YCommandTextBox.Text);
            var zInc = TextToInt(ZCommandTextBox.Text);
            var newCommand = new GCommand(xInc, yInc, zInc, _currentPosePosition);

            _commands.Enqueue(newCommand);

            new Thread(CommandExecutor.SharedInstance.Execute).Start();
        }

        private void SendTouchButton_OnClick(object sender, RoutedEventArgs e)
        {
            _dataContext.AddOutput("Touch Button is clicked!");

            var x = TextToDouble(TouchXTextBox.Text);
            var y = TextToDouble(TouchYTextBox.Text);

            if(y > 0)
            {
                y = -1 * y;
            }

            _testBrain.ReportTouchBegin(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), x, y);

            _dataContext.AddOutput("Touch is reported!");
            Scroller.ScrollToBottom();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            CommandExecutor.SharedInstance.IsStopped = true;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.AddOutput("Reset Button is clicked!");
            var resetCommand = new PoseCommand(0, 0, 0);
            _commands.Enqueue(resetCommand);

            new Thread(CommandExecutor.SharedInstance.Execute).Start();

            Scroller.ScrollToBottom();
        }

        #endregion

        #region Camera
        private void switchCameraBtn_Click(object sender, RoutedEventArgs e)
        {
            if (shouldDetectCamera)
            {
                if ((currentCameraId == maxCameraId) && (currentCameraId >= 2))
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

            if ((_camera != null) && (0 <= currentCameraId))
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
