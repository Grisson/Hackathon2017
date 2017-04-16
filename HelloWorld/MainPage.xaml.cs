using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel ViewModel;
        private SerialDevice serialPort = null;

        public MainPage()
        {
            this.InitializeComponent();

            ViewModel = new MainPageViewModel();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            ComboBoxPort.SelectedIndex = -1;
            ViewModel.ListAvailablePorts();
        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            DeviceInformation entry = (DeviceInformation)ComboBoxPort.SelectedItem;
            if (entry == null)
            {
                ComboBoxPort.SelectedIndex = -1;
                return;
            }

            var selected = (ComboBoxItem) ComboBoxBaud.SelectedItem;
            if (selected == null)
            {
                return;
            }
            uint baud = uint.Parse(selected.Content.ToString());

            try
            {
                serialPort = await SerialDevice.FromIdAsync(entry.Id);
                if (serialPort == null) return;

                // Disable the 'Connect' button 
                //comPortInput.IsEnabled = false;

                // Configure serial settings
                serialPort.WriteTimeout = TimeSpan.FromSeconds(10);
                serialPort.ReadTimeout = TimeSpan.FromSeconds(10);
                serialPort.BaudRate = baud;
                serialPort.Parity = SerialParity.None;
                serialPort.StopBits = SerialStopBitCount.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = SerialHandshake.None;

                // Display configured settings
                //status.Text = "Serial port configured successfully: ";
                //status.Text += serialPort.BaudRate + "-";
                //status.Text += serialPort.DataBits + "-";
                //status.Text += serialPort.Parity.ToString() + "-";
                //status.Text += serialPort.StopBits;

                //// Set the RcvdText field to invoke the TextChanged callback
                //// The callback launches an async Read task to wait for data
                //rcvdText.Text = "Waiting for data...";

                //// Create cancellation token object to close I/O operations when closing the device
                //ReadCancellationTokenSource = new CancellationTokenSource();

                //// Enable 'WRITE' button to allow sending data
                //sendTextButton.IsEnabled = true;

                //Listen();
            }
            catch (Exception ex)
            {
                //status.Text = ex.Message;
                //comPortInput.IsEnabled = true;
                //sendTextButton.IsEnabled = false;
            }
        }
    }
}
