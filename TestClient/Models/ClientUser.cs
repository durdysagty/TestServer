using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TestModels;

namespace TestClient
{
    class ClientUser : User, INotifyPropertyChanged
    {
        private int id;
        private string name;
        private bool isTestPassed;
        private bool isConnected;

        public override int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public override string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public override bool IsTestPassed
        {
            get { return isTestPassed; }
            set
            {
                isTestPassed = value;
                OnPropertyChanged("IsTestPassed");
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            //Console.WriteLine(prop);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
