using CommunityToolkit.Mvvm.Input;
using Model;
using Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private string _fileName;
        private string _searchText;

        private ObjectType _selectedObjectType;
        private Place _selectedPlace;
        private Property _selectedProperty;

        private ObservableCollection<Property> Inventory;

        public MainViewModel()
        {
            if (!File.Exists("inventory.db"))
            {
                DataHandler.CreateDatabase();
                DataHandler.InsertSampleData();
            }

            Inventory = DataHandler.SelectAll();
            Properties = new ObservableCollection<Property>(Inventory);

            Places = DataHandler.SelectPlaces();
            ObjectTypes = DataHandler.SelectObjectTypes();

            ObjectTypes.Insert(0, new ObjectType(0, "Выбрать всё"));
            Places.Insert(0, new Place(0, "Выбрать всё"));

            InsertPlaceCommand = new AsyncRelayCommand(InsertPlace);
            InsertObjectTypeCommand = new AsyncRelayCommand(InsertObjectType);
            InsertObjectCommand = new AsyncRelayCommand(InsertObject);
            InsertPropertyCommand = new AsyncRelayCommand(InsertProperty);
            SearchCommand = new RelayCommand(Search);
            EditCommand = new RelayCommand(Edit);
            DeletePropertyCommand = new AsyncRelayCommand(DeleteProperty);
            InsertUserCommand = new AsyncRelayCommand(InsertUser);
        }

        public IAsyncRelayCommand DeletePropertyCommand { get; set; }

        public string FileName
        {
            get => _fileName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DataHandler.ExportToCSV(value);
                    _fileName = value;
                    OnPropertyChanged();
                }
            }
        }

        public IAsyncRelayCommand InsertObjectCommand { get; set; }
        public IAsyncRelayCommand InsertObjectTypeCommand { get; set; }
        public IAsyncRelayCommand InsertPlaceCommand { get; set; }
        public IAsyncRelayCommand InsertPropertyCommand { get; set; }

        public IAsyncRelayCommand InsertUserCommand { get; set; }

        public IRelayCommand EditCommand { get; set; }

        private void Edit()
        {
            SelectedProperty.IsInStock = !SelectedProperty.IsInStock;
            DataHandler.UpdateInStock(SelectedProperty.PropertyId, SelectedProperty.IsInStock);
        }

        public ObservableCollection<InStock> IsInStock { get; set; } = new ObservableCollection<InStock>
        {
             new InStock(SearchInStock.Nevermind),
             new InStock(SearchInStock.Yes),
             new InStock(SearchInStock.No)
        };

        public ObservableCollection<ObjectType> ObjectTypes { get; set; }
        public ObservableCollection<Place> Places { get; set; } = new();
        public ObservableCollection<Property> Properties { get; set; }
        public IRelayCommand SearchCommand { get; set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }
        }

        private InStock _selectedInStock;

        public InStock SelectedInStock
        {
            get => _selectedInStock;
            set
            {
                if (_selectedInStock != value)
                {
                    _selectedInStock = value;
                    OnPropertyChanged();
                    SelectProperties();
                }
            }
        }

        public ObjectType SelectedObjectType
        {
            get => _selectedObjectType;
            set
            {
                if (_selectedObjectType != value)
                {
                    _selectedObjectType = value;
                    OnPropertyChanged();

                    SelectProperties();
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

                    SelectProperties();
                }
            }
        }

        public Property SelectedProperty
        {
            get => _selectedProperty;
            set
            {
                if (_selectedProperty != value)
                {
                    _selectedProperty = value;
                    OnPropertyChanged();
                }
            }
        }

        private async Task DeleteProperty()
        {
            DataHandler.DeleteProperty(SelectedProperty.PropertyId);
            Properties.Remove(SelectedProperty);
            Inventory.Remove(SelectedProperty);
        }

        private async Task InsertUser()
        {
            CreateUserViewModel createUserViewModel = new();

            await ViewInteraction.ShowModalPresentation(createUserViewModel);
        }

        private async Task InsertObject()
        {
            CreateObjectViewModel createObjectViewModel = new();

            createObjectViewModel.ObjectTypes = new(ObjectTypes);
            createObjectViewModel.ObjectTypes.RemoveAt(0);

            await ViewInteraction.ShowModalPresentation(createObjectViewModel);
        }

        private async Task InsertObjectType()
        {
            CreateObjectTypeViewModel createObjectTypeViewModel = new();
            await ViewInteraction.ShowModalPresentation(createObjectTypeViewModel);

            if (createObjectTypeViewModel.IsConfirm)
            {
                createObjectTypeViewModel.IsConfirm = false;

                ObjectTypes.Add(DataHandler.SelectObjectType(createObjectTypeViewModel.ObjectTypeName));
            }
        }

        private async Task InsertPlace()
        {
            CreatePlaceViewModel createPlaceViewModel = new();
            await ViewInteraction.ShowModalPresentation(createPlaceViewModel);

            if (createPlaceViewModel.IsConfirm)
            {
                createPlaceViewModel.IsConfirm = false;

                Places.Add(DataHandler.SelectPlace(createPlaceViewModel.PlaceName));
            }
        }

        private async Task InsertProperty()
        {
            CreatePropertyViewModel createPropertyViewModel = new();
            createPropertyViewModel.Places = new ObservableCollection<Place>(Places);
            createPropertyViewModel.Places.RemoveAt(0);

            await ViewInteraction.ShowModalPresentation(createPropertyViewModel);

            if (createPropertyViewModel.IsConfirm)
            {
                createPropertyViewModel.IsConfirm = false;

                Property inventory = DataHandler.SelectProperty(createPropertyViewModel.InventoryNumber);

                if ((SelectedObjectType.ObjectTypeId == inventory.ObjectTypeId && SelectedPlace.PlaceId == inventory.PlaceId) | (SelectedObjectType.ObjectTypeId == 0 && SelectedPlace.PlaceId == 0))
                {
                    Properties.Add(inventory);
                }
            }
        }

        private void Search()
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                List<Property> inventories = Properties.Where(inventory => inventory.InventoryNumber.StartsWith(SearchText)).ToList();

                Properties.Clear();

                foreach (Property item in inventories)
                {
                    Properties.Add(item);
                }
            }
        }

        private void SelectProperties()
        {
            SearchText = "";

            if (SelectedObjectType is not null && SelectedPlace is not null && SelectedInStock is not null)
            {
                Properties.Clear();

                if (SelectedObjectType.ObjectTypeId == 0 && SelectedPlace.PlaceId == 0)
                {
                    if (SelectedInStock.SearchInStock == SearchInStock.Nevermind)
                    {
                        foreach (Property item in Inventory)
                        {
                            Properties.Add(item);
                        }
                    }
                    else if (SelectedInStock.SearchInStock == SearchInStock.Yes)
                    {
                        foreach (Property item in Inventory)
                        {
                            if (item.IsInStock)
                            {
                                Properties.Add(item);
                            }
                        }
                    }
                    else
                    {
                        foreach (Property item in Inventory)
                        {
                            if (!item.IsInStock)
                            {
                                Properties.Add(item);
                            }
                        }
                    }
                }
                else if (SelectedObjectType.ObjectTypeId == 0 && SelectedPlace.PlaceId != 0)
                {
                    foreach (Property item in Inventory)
                    {
                        if (item.PlaceId == SelectedPlace.PlaceId)
                        {
                            if (SelectedInStock.SearchInStock == SearchInStock.Nevermind)
                            {
                                Properties.Add(item);
                            }
                            else if (SelectedInStock.SearchInStock == SearchInStock.Yes)
                            {
                                if (item.IsInStock)

                                    Properties.Add(item);
                            }
                        }
                        else
                        {
                            if (!item.IsInStock)
                            {
                                Properties.Add(item);
                            }
                        }
                    }
                }
                else if (SelectedObjectType.ObjectTypeId != 0 && SelectedPlace.PlaceId == 0)
                {
                    foreach (Property item in Inventory)
                    {
                        if (item.ObjectTypeId == SelectedObjectType.ObjectTypeId)
                        {
                            if (SelectedInStock.SearchInStock == SearchInStock.Nevermind)
                            {
                                Properties.Add(item);
                            }
                            else if (SelectedInStock.SearchInStock == SearchInStock.Yes)
                            {
                                if (item.IsInStock)
                                {
                                    Properties.Add(item);
                                }
                            }
                            else
                            {
                                if (!item.IsInStock)
                                {
                                    Properties.Add(item);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (Property item in Inventory)
                    {
                        if (item.ObjectTypeId == SelectedObjectType.ObjectTypeId && item.PlaceId == SelectedPlace.PlaceId)
                        {
                            if (SelectedInStock.SearchInStock == SearchInStock.Nevermind)
                            {
                                Properties.Add(item);
                            }
                            else if (SelectedInStock.SearchInStock == SearchInStock.Yes)
                            {
                                if (item.IsInStock)
                                {
                                    Properties.Add(item);
                                }
                            }
                            else
                            {
                                if (!item.IsInStock)
                                {
                                    Properties.Add(item);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}