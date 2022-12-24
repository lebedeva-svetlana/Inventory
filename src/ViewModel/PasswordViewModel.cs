using CommunityToolkit.Mvvm.Input;
using Model;
using Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public class PasswordViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private bool _isEnabled;
        private bool _isLoginMessageVisible;
        private string _password;
        private string _username;

        public PasswordViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoginMessageVisible
        {
            get => _isLoginMessageVisible;
            set
            {
                if (_isLoginMessageVisible != value)
                {
                    _isLoginMessageVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isPasswordMessageVisible;

        public bool IsPasswordMessageVisible
        {
            get => _isPasswordMessageVisible;
            set
            {
                if (_isPasswordMessageVisible != value)
                {
                    _isPasswordMessageVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public IRelayCommand LoginCommand { get; set; }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value && !string.IsNullOrEmpty(value))
                {
                    _password = value;
                    OnPropertyChanged();
                    Check();
                }
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value && !string.IsNullOrEmpty(value))
                {
                    _username = value;
                    OnPropertyChanged();
                    Check();
                }
            }
        }

        private void Check()
        {
            if (Password != null && Username != null)
            {
                IsEnabled = true;
            }
            else
            {
                IsEnabled = false;
            }
        }

        private void Login()
        {
            string hash = DataHandler.SelectUserByName(Username);

            if (!string.IsNullOrEmpty(hash))
            {
                IsLoginMessageVisible = false;

                PasswordHasher passwordHasher = new(new HashingOptions());

                (bool verified, _) = passwordHasher.Check(hash, Password);

                if (verified)
                {
                    MainViewModel mainViewModel = new();

                    ViewInteraction.ShowPresentation(mainViewModel);

                    ViewInteraction.HidePresentation(this);
                }
                else
                {
                    IsPasswordMessageVisible = true;
                    IsEnabled = false;
                }
            }
            else
            {
                IsPasswordMessageVisible = false;
                IsLoginMessageVisible = true;
                IsEnabled = false;
            }
        }
    }
}