using CommunityToolkit.Mvvm.Input;
using Model;
using Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public class CreatePlaceViewModel : INotifyPropertyChanged
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

        private string _placeName;

        public CreatePlaceViewModel()
        {
            AddPlaceCommand = new RelayCommand(AddPlace);
        }

        public IRelayCommand AddPlaceCommand { get; set; }

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

        public string PlaceName
        {
            get => _placeName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _placeName = value;

                    IsEnabled = true;
                    IsConfirm = true;

                    OnPropertyChanged();
                }
            }
        }


        private void AddPlace()
        {
            ViewInteraction.HidePresentation(this);

            DataHandler.InsertPlace(PlaceName);

            IsConfirm = true;
        }
    }
}