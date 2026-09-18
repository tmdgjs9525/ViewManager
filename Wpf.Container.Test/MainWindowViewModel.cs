using Container.Core.Interfaces;
using EyeContainer.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Wpf.Container.Test
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        public MainWindowViewModel(IDialogService dialogService, INavigationService navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        bool _view = true;
        [RelayCommand]
        private async Task Click()
        {
            if (_view)
            {
                _navigationService.NavigateTo("MainRegion", "BView");
            }
            else
            {
                _navigationService.NavigateTo("MainRegion", "AView");
            }

            _view = !_view;

            var result = await _dialogService.ShowDialogAsync("TestDialog", startPosition: StartPosition.Center);

            if (result is not null && result.Success is true)
            {
                await Task.Run(() => { } );
            }
        }

        [RelayCommand]
        private async Task MainView1()
        {
            _navigationService.NavigateTo("MainRegion", "AView");

            //var result = await _dialogService.ShowDialogAsync("TestDialog", startPosition: StartPosition.Center);

            ////Dialog가 끝나면 아래 코드 호출
            //if (result != null && result.Success)
            //{
            //    await Task.Run(() => { });
            //}
        }
        [RelayCommand]
        private async Task MainView2()
        {

            _navigationService.NavigateTo("MainRegion", "BView");


            //var result = await _dialogService.ShowDialogAsync("TestDialog", startPosition: StartPosition.Center);

            ////Dialog가 끝나면 아래 코드 호출
            //if (result != null && result.Success)
            //{
            //    await Task.Run(() => { });
            //}
        }

        [RelayCommand]
        private async Task GoBack()
        {
            _navigationService.GoBack("MainRegion");
        }
    }
}
