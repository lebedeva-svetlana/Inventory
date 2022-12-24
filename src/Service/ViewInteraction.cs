using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Service
{
    public static class ViewInteraction
    {
        private static readonly Dictionary<object, Window> OpenWindows = new();

        public static Window CreateWindowInstanceWithViewModel(object viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("ViewModel cannot be null");
            }

            Type viewModelType = viewModel.GetType();
            //while (viewModelType is not null && !ViewModelToViewMapping.TryGetValue(viewModelType, out windowType))
            //{
            //    viewModelType = viewModelType.BaseType;
            //}

            ViewRegistration.ViewModelToViewMapping.TryGetValue(viewModelType, out Type windowType);

            if (windowType is null)
            {
                throw new ArgumentException(
                    $"No registered window type for argument type {viewModel.GetType().FullName}");
            }

            Window window = (Window)Activator.CreateInstance(windowType);
            window.DataContext = viewModel;
            return window;
        }

        public static void ShowPresentation(object viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException("ViewModel cannot be null");
            }

            if (OpenWindows.ContainsKey(viewModel))
            {
                throw new InvalidOperationException("UI for this ViewModel is already displayed");
            }

            Window window = CreateWindowInstanceWithViewModel(viewModel);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
            OpenWindows[viewModel] = window;
        }

        public static void HidePresentation(object viewModel)
        {
            if (!OpenWindows.TryGetValue(viewModel, out Window window))
            {
                throw new InvalidOperationException("UI for this ViewModel is not displayed");
            }

            window.Close();
            OpenWindows.Remove(viewModel);
        }

        public static async Task ShowModalPresentation(object viewModel)
        {
            Window window = CreateWindowInstanceWithViewModel(viewModel);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            OpenWindows[viewModel] = window;
            await window.Dispatcher.InvokeAsync(() => window.ShowDialog());
        }
    }
}