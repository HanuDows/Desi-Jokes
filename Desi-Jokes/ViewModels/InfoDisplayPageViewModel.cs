using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace Desi_Jokes.ViewModels
{
    public class InfoDisplayPageViewModel : ViewModelBase
    {
        public InfoDisplayPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            }
        }

        private string _infoContent = "Initial Content";
        public string InfoContent { get { return _infoContent; } set { Set(ref _infoContent, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string title = localSettings.Values["ToastMessageTitle"].ToString();
            string content = localSettings.Values["ToastMessageContent"].ToString();

            InfoContent = title + "\n\r" + content;

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }
    }
}

