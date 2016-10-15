using System;
using Desi_Jokes.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using HanuDowsFramework;
using Windows.UI.Core;

namespace Desi_Jokes.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

            //SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            //var type = e.GetType();
        }
    }
}
