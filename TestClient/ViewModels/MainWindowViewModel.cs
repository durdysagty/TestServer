using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using TestClient.Commands;
using TestClient.Models;
using TestClient.Services;
using TestModels;
using TestModels.Interfaces;
using TestModels.Services;

namespace TestClient
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ClientUser _clientUser;
        private readonly IServerAction _serverAction;
        public ClientUser ClientUser
        {
            get { return _clientUser; }
        }
        private string buttonName;
        public string ButtonName
        {
            get => buttonName;
            set
            {
                if (buttonName != value)
                {
                    buttonName = value;
                    OnPropertyChanged();
                }
            }
        }
        private string textBoxColor;
        public string TextBoxColor
        {
            get => textBoxColor;
            set
            {
                if (textBoxColor != value)
                {
                    textBoxColor = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool enableLogin;
        public bool EnableLogin
        {
            get => enableLogin;
            set
            {
                if (enableLogin != value)
                {
                    enableLogin = value;
                    OnPropertyChanged();
                }
            }
        }
        private readonly RelayCommand _connect;
        public RelayCommand Connect
        {
            get { return _connect; }
        }

        private readonly RelayCommand _getTest;
        public RelayCommand GetTest
        {
            get { return _getTest; }
        }

        private bool isTestStarted;
        public bool IsTestStarted
        {
            get { return isTestStarted; }
            set
            {
                isTestStarted = value;
                OnPropertyChanged();
            }
        }

        private Test test;
        public Test Test
        {
            get { return test; }
            set
            {
                test = value;
                OnPropertyChanged();
            }
        }

        private string already;
        public string Already
        {
            get { return already; }
            set
            {
                already = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<int> answers;
        public ObservableCollection<int> Answers
        {
            get { return answers; }
            set
            {
                answers = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TestImage> imageAnswers;
        public ObservableCollection<TestImage> ImageAnswers
        {
            get { return imageAnswers; }
            set
            {
                imageAnswers = value;
                OnPropertyChanged();
            }
        }

        private string _sequenceAnswer0;
        public string SequenceAnswer0
        {
            get { return _sequenceAnswer0; }
            set
            {
                if (_sequenceAnswer0 != value)
                {
                    _sequenceAnswer0 = value;
                    Answers[0] = int.Parse(value);
                    OnPropertyChanged();
                }
            }
        }

        private string _sequenceAnswer1;
        public string SequenceAnswer1
        {
            get { return _sequenceAnswer1; }
            set
            {
                if (_sequenceAnswer1 != value)
                {
                    _sequenceAnswer1 = value;
                    Answers[1] = int.Parse(value);
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand2 addAnswer;
        public RelayCommand2 AddAnswer
        {
            get { return addAnswer; }
            set { addAnswer = value;  }
        }

        private readonly RelayCommand _sendTest;
        public RelayCommand SendTest
        {
            get { return _sendTest; }
        }

        private List<TestImage> _images;
        public List<TestImage> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                OnPropertyChanged();
            }
        }
        public MainWindowViewModel()
        {
            _clientUser = new ClientUser();
            _serverAction = new ServerActionService();
            ButtonName = "Подключиться";
            TextBoxColor = "LightGray";
            EnableLogin = true;
            _connect = new RelayCommand(async () =>
            {
                if (_clientUser.IsConnected)
                {
                    ServerConnection.Disconnect();
                    ButtonName = "Подключиться";
                    //_clientUser.Name = null;
                    _clientUser.IsConnected = false;
                    EnableLogin = true;
                    IsTestStarted = false;
                }
                else
                {
                    if (_clientUser.Name != null && _clientUser.Name != string.Empty)
                    {
                        TextBoxColor = "LightGray";
                        bool isConnected = await ServerConnection.ConnectAsync();
                        if (isConnected)
                        {
                            _ = Task.Run(() => ServerConnection.ReceiveData());
                            ServerAction serverAction = _serverAction.CreateAction(EnableAction.Login, _clientUser.Name);
                            string result = await ServerConnection.SendData(serverAction);
                            if (result != null)
                            {
                                bool isConverted = bool.TryParse(result, out bool isPassed);
                                if (isConverted)
                                {
                                    Already = null;
                                    _clientUser.IsTestPassed = isPassed;
                                    ButtonName = "Отключиться";
                                    EnableLogin = false;
                                    _clientUser.IsConnected = true;
                                }
                                else
                                {
                                    Already = result;
                                    ServerConnection.Disconnect();
                                }
                            }
                        }
                    }
                    else
                    {
                        TextBoxColor = "Red";
                    }
                }
            });
            _getTest = new RelayCommand(async () =>
            {
                ServerAction serverAction = _serverAction.CreateAction(EnableAction.Test, _clientUser.Name);
                string result = await ServerConnection.SendData(serverAction);
                if (result != null)
                {
                    IsTestStarted = true;
                    Test = JsonSerializer.Deserialize<Test>(result);
                    switch (Test.TestType)
                    {
                        case TestType.MultipleChoice:
                            Answers = new ObservableCollection<int>();
                            AddAnswer = new RelayCommand2((number) =>
                            {
                                int a = (int)number;
                                Answers.Add(a);
                                OnPropertyChanged(nameof(Answers));
                            });
                            break;
                        case TestType.Sequence:
                            Answers = new ObservableCollection<int>
                            {
                                0,
                                1
                            };
                            AddAnswer = new RelayCommand2((number) =>
                            {
                                int a = int.Parse((string)number);
                                Answers.Add(a);
                                OnPropertyChanged(nameof(Answers));
                            });
                            break;
                        default:
                            int n = 1;
                            ImageAnswers = new ObservableCollection<TestImage>();
                            List<TestImage> bitmapImages = Test.Images.Select(i => new TestImage()
                            {
                                Order = n++,
                                Image = ImageConvertor.ByteArrayToImage(i)
                            }).ToList();
                            Images = bitmapImages;
                            AddAnswer = new RelayCommand2((image) =>
                            {
                                TestImage testImage = image as TestImage;
                                ImageAnswers.Add(testImage);
                                OnPropertyChanged(nameof(ImageAnswers));
                            });
                            break;
                    }
                }
            });
            _sendTest = new RelayCommand(async () =>
            {
                UserAnswer userAnswer = new()
                {
                    Name = _clientUser.Name
                };
                if (Test.TestType == TestType.SelectImages)
                {
                    IList<byte[]> bytes = imageAnswers.Select(ia => ImageConvertor.ImageToByteArray(ia.Image)).ToList();
                    userAnswer.Images = bytes;
                }
                else
                    userAnswer.Answer = answers;
                ServerAction serverAction = _serverAction.CreateAction(EnableAction.Check, userAnswer);
                string result = await ServerConnection.SendData(serverAction);
                if (result != null)
                {
                    bool isPassed = JsonSerializer.Deserialize<bool>(result);
                    _clientUser.IsTestPassed = isPassed;
                    IsTestStarted = false;
                    Answers = new ObservableCollection<int>();
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
