using System;
using System.Collections.Generic;
using System.Windows;

namespace Service
{
    public static class ViewRegistration
    {
        internal static readonly Dictionary<Type, Type> ViewModelToViewMapping = new();

        public static void RegisterViewType<TViewModel, TView>() where TView : Window, new() where TViewModel : class
        {
            Type viewModelType = typeof(TViewModel);
            if (viewModelType.IsInterface)
            {
                throw new ArgumentException("Cannot register interfaces");
            }

            if (ViewModelToViewMapping.ContainsKey(viewModelType))
            {
                throw new InvalidOperationException(
                    $"Type {viewModelType.FullName} is already registered");
            }

            ViewModelToViewMapping[viewModelType] = typeof(TView);
        }

        public static void UnregisterViewType<TViewModel>()
        {
            Type viewModelType = typeof(TViewModel);
            if (viewModelType.IsInterface)
            {
                throw new ArgumentException("Cannot register interfaces");
            }

            if (!ViewModelToViewMapping.ContainsKey(viewModelType))
            {
                throw new InvalidOperationException(
                    $"Type {viewModelType.FullName} is not registered");
            }

            ViewModelToViewMapping.Remove(viewModelType);
        }
    }
}