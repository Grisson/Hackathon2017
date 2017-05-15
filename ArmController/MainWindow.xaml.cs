﻿using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ArmController.lib;
using ArmController.lib.Data;
using System.Collections.Generic;
using System.Threading;
using Emgu.CV.UI;
using Newtonsoft.Json;
using ArmController.Executor;

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
        private PosePosition _currentPosePosition;

        private readonly TestRunner _testBrain; // assume this is the cloud
        private CommandStore _commands => CommandStore.SharedInstance; // from cloud or human inputs
        private BaseCommand _currentCommand => CommandStore.SharedInstance.CurrentCommand;
        private SerialCommunicator _serialPort
        {
            get
            {
                return GCommandExecutor.SharedInstance.SerialPort;
            }
            set
            {
                GCommandExecutor.SharedInstance.SerialPort = value;
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

            _testBrain = new TestRunner();
            _testBrain.RegisterTestTarget();

            _deviceId = Guid.NewGuid();
            _currentPosePosition = PosePosition.InitializePosition();

            DataContext = _dataContext;
            ComboBoxBaud.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = _baudList });
            ComboBoxBaud.SelectedIndex = 5;
        }



        //private void ExcuteCommand()
        //{
        //    if ((_serialPort == null) || !_serialPort.IsConnected)
        //    {
        //        return;
        //    }

        //    var continueToExcute = false;
        //    if (!IsWaitingResponse)
        //    {
        //        lock (this)
        //        {
        //            if (!IsWaitingResponse && _commands.Count > 0)
        //            {
        //                IsWaitingResponse = true;
        //                continueToExcute = true;
        //            }
        //        }
        //    }

        //    if (!continueToExcute)
        //    {
        //        return;
        //    }

        //    if (_commands.TryDequeue(out _currentCommand))
        //    {
        //        var tmpCommand = (GCommand)_currentCommand;
        //        tmpCommand.SendTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        //        _serialPort.WriteLine(tmpCommand.CommandText);
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            _dataContext.AddOutput(tmpCommand.ToSendLog());
        //        });

        //    }
        //    else
        //    {
        //        lock (this)
        //        {
        //            IsWaitingResponse = false;
        //        }
        //    }
        //}

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var sp = (SerialPort)sender;
                while (sp.BytesToRead > 0)
                {
                    var d = sp.ReadLine();

                    if (_currentCommand != null)
                    {
                        var tmpCommand = _currentCommand as GCommand;
                        tmpCommand.Receive(d);
                        _dataContext.AddOutput(tmpCommand.ToReceiveLog());
                        if (d.Equals("OK\r", StringComparison.InvariantCultureIgnoreCase))
                        {
                            CommandComplete(tmpCommand);
                        }
                    }
                }
                Scroller.ScrollToBottom();

            });
        }

        private void CommandComplete(GCommand currentCommand)
        {
            // update current test agent pose position
            var nextP = currentCommand.NextPosePosition;
            if (nextP != null)
            {
                _currentPosePosition = nextP;
                ShowCurrentPosition();
            }

            // report pose position
            _testBrain.ReportAgentPosePosition(currentCommand.SendTimeStamp,
                _currentPosePosition.X,
                _currentPosePosition.Y,
                _currentPosePosition.Z);

            CommandStore.SharedInstance.CurrentCommand = null;

            lock (CommandExecutor.SharedInstance)
            {
                IsWaitingResponse = false;
            }

            new Thread(CommandExecutor.SharedInstance.Execute).Start();
        }

        private void GetCommandsFromServer()
        {
            var commandsText = _testBrain.GetCalibrationCommonds();

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            List<BaseCommand> deserializedList = JsonConvert.DeserializeObject<List<BaseCommand>>(commandsText, settings);

            foreach(var c in deserializedList)
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
    }
}