using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController
{
    public class ConsoleContent : INotifyPropertyChanged
    {
        string _consoleInput = string.Empty;
        ObservableCollection<string> _consoleOutput = new ObservableCollection<string>() { "Output..." };

        public string ConsoleInput
        {
            get
            {
                return _consoleInput;
            }
            set
            {
                _consoleInput = value;
                OnPropertyChanged("ConsoleInput");
            }
        }

        public ObservableCollection<string> ConsoleOutput
        {
            get
            {
                return _consoleOutput;
            }
            set
            {
                _consoleOutput = value;
                OnPropertyChanged("ConsoleOutput");
            }
        }

        public void RunCommand()
        {
            ConsoleOutput.Add(ConsoleInput);
            // do your stuff here.
            ConsoleInput = String.Empty;
        }

        public void AddOutput(string newOutput)
        {
            ConsoleOutput.Add(newOutput);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
