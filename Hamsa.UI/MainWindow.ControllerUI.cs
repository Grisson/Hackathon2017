using Hamsa.Device;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hamsa.UI
{
    public partial class MainWindow
    {
        public ThreeDOFArm controlArm { get; set; }

        private void AbsoluteGoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (((controlArm != null) && controlArm.IsConnected))
            {
                var x = TextToInt(AbsoluteXTextBox.Text);
                var y = TextToInt(AbsoluteYTextBox.Text);
                var z = TextToInt(AbsoluteZTextBox.Text);

                var pose = new PosePosition(x, y, z);

                controlArm.MoveTo(pose);
            }
        }

        private void RelativeGoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (((controlArm != null) && controlArm.IsConnected))
            {
                var xInc = TextToInt(RelativeXTextBox.Text);
                var yInc = TextToInt(RelativeYTextBox.Text);
                var zInc = TextToInt(RelativeZTextBox.Text);

                var currentPosition = controlArm.GetLatestData();
                var targetPose = new PosePosition(currentPosition.X + xInc,
                    currentPosition.Y + yInc, currentPosition.Z + zInc);

                controlArm.MoveTo(targetPose);
            }
        }

        private void CoordinateGoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (((controlArm != null) && controlArm.IsConnected))
            {
                var coorX = TextToDouble(CoordinateXTextBox.Text);
                var coorY = TextToDouble(CoordinateYTextBox.Text);
                var coorZ = TextToDouble(CoordinateZTextBox.Text);

                var pose = controlArm.ConvertToPose(new Tuple<double, double, double>(coorX, coorY, coorZ));

                controlArm.MoveTo(pose);
            }
        }

        public void ShowControlArmPosition(string data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var currentPose = this.controlArm.GetLatestData();
                ControllerShowCurrentStatus(currentPose);
            });
        }

        private void ArmConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(ArmConnectButton.Content.ToString(), "Disconntect", StringComparison.CurrentCultureIgnoreCase))
            {
                if (((controlArm != null) && controlArm.IsConnected))
                {
                    controlArm.ResetPosePosition();
                    controlArm.Close();
                    controlArm.Dispose();
                    controlArm = null;
                    ArmConnectButton.Content = "Conntect";
                }
            }
            else
            {
                if (controlArm == null)
                {
                    var portName = PortComboBox.SelectedValue.ToString();
                    var baud = 115200;
                    controlArm = new ThreeDOFArm(portName, baud);
                    controlArm.Subscript(string.Empty, ShowControlArmPosition);
                }

                if (!controlArm.IsConnected)
                {
                    controlArm.Connect();
                }

                if (controlArm.IsConnected)
                {
                    ArmConnectButton.Content = "Disconntect";
                    var currentPose = controlArm.GetLatestData();
                    ControllerShowCurrentStatus(currentPose);
                }
            }
        }

        private void ControllerShowCurrentStatus(PosePosition pose)
        {
            CurrentPoseLabel.Content = $"Current Pose X:{pose.X} Y:{pose.Y} z:{pose.Z}";
            var coor = controlArm.ConvertToCoordinate(pose);
            CurrentCoordinateLabel.Content = $"Current Coordinate X:{coor.Item1} Y:{coor.Item2} Z:{coor.Item3}";
        }

        private void PortComboBox_DropDownOpened(object sender, EventArgs e)
        {
            PortComboBox.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = SerialPort.GetPortNames() });
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

        private int TextToInt(string txt)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(txt))
            {
                int.TryParse(txt, out result);
            }

            return result;
        }
    }
}
