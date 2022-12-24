using CommunityToolkit.Mvvm.Input;
using Model;
using Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ViewModel
{
    public class CreateUserViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private static Regex ValidatePassword = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");

        private bool _isEnabled;
        private bool _isVisible;
        private string _password;
        private string _username;
        private bool IsLoginOk;
        private bool IsPasswordOk;

        private List<string> Usernames = DataHandler.SelectUsernames();

        public CreateUserViewModel()
        {
            CreateUserCommand = new RelayCommand(CreateUser);
        }

        public IRelayCommand CreateUserCommand { get; set; }

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

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (ValidatePassword.IsMatch(value))
                {
                    _password = value;
                    IsPasswordOk = true;

                    OnPropertyChanged();
                    Check();
                }
                else
                {
                    IsPasswordOk = false;
                }
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Usernames.Contains(value))
                    {
                        IsLoginOk = false;
                        IsVisible = true;
                    }
                    else
                    {
                        IsVisible = false;
                        _username = value;
                        IsLoginOk = true;

                        OnPropertyChanged();
                        Check();
                    }
                }
                else
                {
                    IsLoginOk = false;
                }
            }
        }

        private void Check()
        {
            if (IsPasswordOk && IsLoginOk)
            {
                IsEnabled = true;
            }
            else
            {
                IsEnabled = false;
            }
        }

        public bool IsAcepted { get; set; }

        private void CreateUser()
        {
            if (Usernames.Any())
            {
                ViewInteraction.HidePresentation(this);
            }

            PasswordHasher passwordHasher = new(new HashingOptions());

            string hash = passwordHasher.Hash(Password);

            DataHandler.InsertUser(Username, hash);

            IsAcepted = true;


        }
    }
}