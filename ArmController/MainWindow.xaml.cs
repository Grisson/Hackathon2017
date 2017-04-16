using System;
using System.Globalization;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ArmController.lib;
using ArmController.lib.Data;
using System.Collections.Generic;

namespace ArmController
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int[] _baudList = {9600, 19200, 38400, 57600, 74880, 115200, 230400, 250000};
        private readonly ConsoleContent _dataContext = new ConsoleContent();

        private bool _isWaitingResponse;

        private readonly TestRunner _testBrain;
        private Queue<Command> _commands;
        private Command _currentCommand;
        private PosePosition _currentPosePosition;
        private Guid _deviceId;
        private SerialCommunicator _serialPort;

        public PosePosition CurrentPosePosition => _currentPosePosition;

        public bool IsWaitingResponse
        {
            get { return _isWaitingResponse; }
            set
            {
                if (value)
                {
                    // disable UI control
                    DisableUI();
                }
                else
                {
                    // re-enable UI
                    EnableUI();
                }
                _isWaitingResponse = value;

            }
        }

        public MainWindow()
        {
            InitializeComponent();

            _commands = new Queue<Command>();
            _testBrain = new TestRunner();
            _testBrain.RegisterTestTarget();

            _deviceId = Guid.NewGuid();
            _currentPosePosition = PosePosition.InitializePosition();

            DataContext = _dataContext;
            ComboBoxBaud.SetBinding(ItemsControl.ItemsSourceProperty, new Binding {Source = _baudList});
            ComboBoxBaud.SelectedIndex = 5;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var sp = (SerialPort) sender;
                while (sp.BytesToRead > 0)
                {
                    var d = sp.ReadLine();

                    if (_currentCommand != null)
                    {
                        _currentCommand.Receive(d);
                        _dataContext.AddOutput(_currentCommand.ToReceiveLog());
                        if (d.Equals("OK\r", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var nextP = _currentCommand.NextPosePosition;
                            if (nextP != null)
                            {
                                _currentPosePosition = nextP;
                                ShowCurrentPosition();
                                IsWaitingResponse = false;
                            }
                            _currentCommand = null;
                        }
                    }

                    Scroller.ScrollToBottom();
                }
            });
        }

        private void ComboBoxPort_OnDropDownOpened(object sender, EventArgs e)
        {
            ComboBoxPort.SetBinding(ItemsControl.ItemsSourceProperty, new Binding {Source = SerialPort.GetPortNames()});
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
            var newCommand = new Command(xInc, yInc, zInc, _currentPosePosition)
            {
                SendTimeStamp = DateTime.UtcNow
            };

            _commands.Enqueue(newCommand);
        }

        private void SendTouchButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private double TextToDouble(string txt)
        {
            double result = 0;
            if (!string.IsNullOrEmpty(txt))
            {
                double.TryParse(txt, out result);
            }

            return result;
        }

        private void ShowCurrentPosition()
        {
            if (_currentPosePosition != null)
            {
                CurrentPositionX.Text = $"X: {_currentPosePosition.X.ToString(CultureInfo.InvariantCulture)}";
                CurrentPositionY.Text = $"Y: {_currentPosePosition.Y.ToString(CultureInfo.InvariantCulture)}";
                CurrentPositionZ.Text = $"Z: {_currentPosePosition.Z.ToString(CultureInfo.InvariantCulture)}";
            }
        }

        private void DisableUI()
        {
            SendCommandButton.IsEnabled = false;
            ConnectButton.IsEnabled = false;
            XCommandTextBox.IsEnabled = false;
            YCommandTextBox.IsEnabled = false;
            ZCommandTextBox.IsEnabled = false;
        }

        private void EnableUI()
        {
            SendCommandButton.IsEnabled = true;
            ConnectButton.IsEnabled = true;
            XCommandTextBox.IsEnabled = true;
            YCommandTextBox.IsEnabled = true;
            ZCommandTextBox.IsEnabled = true;
        }

        private void ExcuteCommond()
        {
            if (!_serialPort.IsConnected)
            {
                return;
            }

            var xInc = TextToDouble(XCommandTextBox.Text);
            var yInc = TextToDouble(YCommandTextBox.Text);
            var zInc = TextToDouble(ZCommandTextBox.Text);
            _currentCommand = new Command(xInc, yInc, zInc, _currentPosePosition)
            {
                SendTimeStamp = DateTime.UtcNow
            };
            IsWaitingResponse = true;

            _serialPort.WriteLine(_currentCommand.CommandText);
            _dataContext.AddOutput(_currentCommand.ToSendLog());
        }
    }
}