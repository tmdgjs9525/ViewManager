using EyeContainer.Core.Interfaces;
using EyeContainer.Core.Parameter;
using EyeEyeContainer.Core.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Avalonia.Container.Test.ViewModels
{
    public partial class TestDialogViewModel : ViewModelBase, IDialogAware
    {
        public string? Title { get; set; }

        public event Action<IDialogResult?>? RequestClose;


        [RelayCommand]
        private void Exit()
        {
            RequestClose?.Invoke(new DialogResult { Success = true, Parameters = new Parameters() });
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(Parameters parameters)
        {
            
        }
    }
}
