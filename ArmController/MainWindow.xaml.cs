using ArmController.Executor;
using ArmController.Models.Command;
using ArmController.Models.Data;
using ArmController.REST;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArmController
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int[] _baudList = { 9600, 19200, 38400, 57600, 74880, 115200, 230400, 250000 };
        private readonly ConsoleContent _dataContext = new ConsoleContent();



        private Guid _deviceId;
        private PosePosition _currentPosePosition
        {
            get
            {
                return CommandStore.SharedInstance.CurrentPosePosition;
            }

            set
            {
                CommandStore.SharedInstance.CurrentPosePosition = value;
            }
        }

        private CloudBrain _testBrain => CommandExecutor.SharedInstance.brain; // assume this is the cloud
        private CommandStore _commands => CommandStore.SharedInstance; // from cloud or human inputs
        private BaseCommand _currentCommand => CommandStore.SharedInstance.CurrentCommand;
        private SerialCommunicator _serialPort
        {
            get
            {
                return CommandExecutor.SharedInstance.SerialPort;
            }
            set
            {
                CommandExecutor.SharedInstance.SerialPort = value;
            }
        }

        private bool IsWaitingResponse
        {
            get
            {
                return CommandExecutor.SharedInstance.IsWaitingResponse;
            }
            set
            {
                CommandExecutor.SharedInstance.IsWaitingResponse = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            //_testBrain = new TestRunner();
            //_testBrain.RegisterTestTarget();

            _deviceId = Guid.NewGuid();
            _currentPosePosition = PosePosition.InitializePosition();

            DataContext = _dataContext;
            ComboBoxBaud.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = _baudList });
            ComboBoxBaud.SelectedIndex = 5;

            CommandExecutor.SharedInstance.LogHandler = ShowLog;
        }

        public void ShowLog(string log)
        {
            Application.Current.Dispatcher.Invoke(() =>
                {
                    _dataContext.AddOutput(log);
                });
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            CommandExecutor.SharedInstance.Callback(sender, string.Empty, _currentCommand);

            Application.Current.Dispatcher.Invoke(() =>
            {
                ShowCurrentPosition();
                Scroller.ScrollToBottom();
            });

            new Thread(CommandExecutor.SharedInstance.Execute).Start();
        }

        private void GetCommandsFromServer()
        {
            var commandsText = _testBrain.Arm.StartCalibrate(CommandExecutor.SharedInstance.RegisterId.Value); ; //.GetSecondCalibrationCommonds();

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            List<BaseCommand> deserializedList = JsonConvert.DeserializeObject<List<BaseCommand>>(commandsText, settings);

            foreach (var c in deserializedList)
            {
                _commands.Enqueue(c);
            }

            new Thread(CommandExecutor.SharedInstance.Execute).Start();
        }

        private void GetProbCommandsFromServer()
        {
            var commandsText = _testBrain.Arm.Prob(CommandExecutor.SharedInstance.RegisterId.Value, 0); //.GetProbCommands();

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            List<BaseCommand> deserializedList = JsonConvert.DeserializeObject<List<BaseCommand>>(commandsText, settings);

            foreach (var c in deserializedList)
            {
                _commands.Enqueue(c);
            }

            new Thread(CommandExecutor.SharedInstance.Execute).Start();
        }

        #region Utilities

        private double TextToDouble(string txt)
        {
            double result = 0;
            if (!string.IsNullOrEmpty(txt))
            {
                double.TryParse(txt, out result);
            }

            return result;
        }

        private int TextToInt(string txt)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(txt))
            {
                int.TryParse(txt, out result);
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

                var coordinate = _testBrain.Arm.ConvertPoseToCoordinate(
                    CommandExecutor.SharedInstance.RegisterId.Value,
                    _currentPosePosition.X,
                    _currentPosePosition.Y,
                    _currentPosePosition.Z) as JArray;
                CurrentCoordinateX.Text = $"X: {coordinate[0]}";
                CurrentCoordinateY.Text = $"Y: {coordinate[1]}";
                CurrentCoordinateZ.Text = $"Z: {coordinate[2]}";
            }
        }

        private void DisableUI()
        {
            //SendCommandButton.IsEnabled = false;
            //ConnectButton.IsEnabled = false;
            //XCommandTextBox.IsEnabled = false;
            //YCommandTextBox.IsEnabled = false;
            //ZCommandTextBox.IsEnabled = false;
        }

        private void EnableUI()
        {
            //SendCommandButton.IsEnabled = true;
            //ConnectButton.IsEnabled = true;
            //XCommandTextBox.IsEnabled = true;
            //YCommandTextBox.IsEnabled = true;
            //ZCommandTextBox.IsEnabled = true;
        }

        #endregion

        [Obsolete]
        private void CommandComplete(PosePosition nextP, long sendTimeStamp)
        {
            // update current test agent pose position
            if (nextP != null)
            {
                _currentPosePosition = nextP;
                ShowCurrentPosition();
            }

            // report pose position
            _testBrain.Arm.ReportPose(
                CommandExecutor.SharedInstance.RegisterId.Value,
                sendTimeStamp.ToString(),
                _currentPosePosition.X,
                _currentPosePosition.Y,
                _currentPosePosition.Z);

            CommandStore.SharedInstance.CurrentCommand = null;

            lock (CommandExecutor.SharedInstance)
            {
                IsWaitingResponse = false;
            }

        }

        private void TestTouchButton_Click(object sender, RoutedEventArgs e)
        {
            //var commandsText = _testBrain.GetTestTouchCommand();

            //JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            //List<BaseCommand> deserializedList = JsonConvert.DeserializeObject<List<BaseCommand>>(commandsText, settings);

            //foreach (var c in deserializedList)
            //{
            //    _commands.Enqueue(c);
            //}

            //new Thread(CommandExecutor.SharedInstance.Execute).Start();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLog("Register Button is clicked!");
            CommandExecutor.SharedInstance.Register();
            ShowLog($"Devic is registed as {CommandExecutor.SharedInstance.RegisterId}");
        }

        
    }
}