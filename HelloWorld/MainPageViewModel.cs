using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation.Diagnostics;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HelloWorld
{
    class MainPageViewModel
    {
        public ObservableCollection<DeviceInformation> ListOfDevices = new ObservableCollection<DeviceInformation>();

        public MainPageViewModel()
        {
            ListAvailablePorts();
        }


        public async void ListAvailablePorts()
        {
            if (ListOfDevices == null)
            {
                ListOfDevices = new ObservableCollection<DeviceInformation>();
            }

            if (ListOfDevices.Count > 0)
            {
                ListOfDevices.Clear();
            }

            string aqs = SerialDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(aqs);

            foreach (var di in dis)
            {
                ListOfDevices.Add(di);
            }
        }
    }
}
