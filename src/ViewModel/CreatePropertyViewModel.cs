using CommunityToolkit.Mvvm.Input;
using Model;
using Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Object = Model.Object;

namespace ViewModel
{
    public class CreatePropertyViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private string _inventoryNumber;
        private bool _isEnabled;
        private Object _selectedObject;
        private Place _selectedPlace;

        public CreatePropertyViewModel()
        {
            Objects = DataHandler.SelectObjects();

            AddPropertyCommand = new RelayCommand(AddProperty);
        }

        public IRelayCommand AddPropertyCommand { get; set; }

        public string InventoryNumber
        {
            get => _inventoryNumber;
            set
            {
                if (_inventoryNumber != value && !string.IsNullOrEmpty(value))
                {
                    _inventoryNumber = value;
                    IsEnabled = true;
                    OnPropertyChanged();
                }
            }
        }

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

        public ObservableCollection<Object> Objects { get; set; }
        public ObservableCollection<Place> Places { get; set; }

        public Object SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    OnPropertyChanged();
                }
            }
        }

        public Place SelectedPlace
        {
            get => _selectedPlace;
            set
            {
                if (_selectedPlace != value)
                {
                    _selectedPlace = value;
                    OnPropertyChanged();
                }
            }
        }

        private void AddProperty()
        {
            ViewInteraction.HidePresentation(this);

            DataHandler.InsertProperty(SelectedObject.ObjectId, SelectedPlace.PlaceId, InventoryNumber);

            IsConfirm = true;
        }
    }
}