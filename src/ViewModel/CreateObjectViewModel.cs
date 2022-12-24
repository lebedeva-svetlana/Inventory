using CommunityToolkit.Mvvm.Input;
using Model;
using Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public class CreateObjectViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private string _description;

        private ObjectType _selectedObjectType;

        public CreateObjectViewModel()
        {
            AddObjectCommand = new RelayCommand(AddObject);
        }

        public IRelayCommand AddObjectCommand { get; set; }

        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsConfirm { get; set; }

        public ObservableCollection<ObjectType> ObjectTypes { get; set; }

        public ObjectType SelectedObjectType
        {
            get => _selectedObjectType;
            set
            {
                if (_selectedObjectType != value)
                {
                    _selectedObjectType = value;
                    OnPropertyChanged();
                }
            }
        }

        private void AddObject()
        {
            ViewInteraction.HidePresentation(this);

            DataHandler.InsertObject(SelectedObjectType.ObjectTypeId, Description);

            IsConfirm = true;
        }
    }
}