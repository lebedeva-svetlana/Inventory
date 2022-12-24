using Model;
using Service;
using System.IO;
using System.Windows;
using View;
using ViewModel;

namespace Inventory
{
    public partial class App : Application
    {
        public App()
        {
            ViewRegistration.RegisterViewType<PasswordViewModel, PasswordView>();
            ViewRegistration.RegisterViewType<MainViewModel, MainView>();
            ViewRegistration.RegisterViewType<CreatePlaceViewModel, CreatePlaceView>();
            ViewRegistration.RegisterViewType<CreateObjectTypeViewModel, CreateObjectTypeView>();
            ViewRegistration.RegisterViewType<CreateObjectViewModel, CreateObjectView>();
            ViewRegistration.RegisterViewType<CreatePropertyViewModel, CreatePropertyView>();
            ViewRegistration.RegisterViewType<CreateUserViewModel, CreateUserView>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!File.Exists("inventory.db"))
            {
                DataHandler.CreateDatabase();
                DataHandler.InsertSampleData();
            }

            bool haveUsers = DataHandler.HaveUsers();

            if (!haveUsers)
            {
                CreateUserViewModel createUserViewModel = new();
                await ViewInteraction.ShowModalPresentation(createUserViewModel);
                if (createUserViewModel.IsAcepted)
                {
                    MainViewModel mainViewModel = new();
                    ViewInteraction.ShowPresentation(mainViewModel);
                    ViewInteraction.HidePresentation(createUserViewModel);
                }
            }
            else
            {
                PasswordViewModel passwordViewModel = new();
                await ViewInteraction.ShowModalPresentation(passwordViewModel);
            }
        }
    }
}