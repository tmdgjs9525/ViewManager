using EyeContainer.Core.Interfaces;
using EyeEyeContainer.Core.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Avalonia.Container.Test.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public string Greeting { get; } = "Welcome to Avalonia!";

        public MainWindowViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

        }

        private bool _view = true;
        [RelayCommand]
        private async Task MainView1()
        {
            //_navigationService.NavigateTo("MainRegion", "MainView1");

            var result = await _dialogService.ShowDialogAsync("TestDialog", startPosition: StartPosition.Center);

            //Dialog가 끝나면 아래 코드 호출
            if (result != null && result.Success)
            {
                await Task.Run(() => { });
            }
        }
        [RelayCommand]
        private async Task MainView2()
        {
            
            _navigationService.NavigateTo("MainRegion", "MainView2");
            

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
