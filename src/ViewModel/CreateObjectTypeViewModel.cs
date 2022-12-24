using CommunityToolkit.Mvvm.Input;
using Model;
using Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public class CreateObjectTypeViewModel : INotifyPropertyChanged
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

        private string _objectTypeName;

        public CreateObjectTypeViewModel()
        {
            AddObjectTypeNameCommand = new RelayCommand(AddObjectType);
        }

        public IRelayCommand AddObjectTypeNameCommand { get; set; }

        public bool IsConfirm { get; set; }

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

        public string ObjectTypeName
        {
            get => _objectTypeName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _objectTypeName = value;

                    IsEnabled = true;
                    IsConfirm = true;

                    OnPropertyChanged();
                }
            }
        }

        private void AddObjectType()
        {
            ViewInteraction.HidePresentation(this);

            DataHandler.InsertObjectType(ObjectTypeName);

            IsConfirm = true;
        }
    }
}