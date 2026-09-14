using EyeContainer.Core.Interfaces;
using EyeContainer.Core.Parameter;
using EyeEyeContainer.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Wpf.Container.Test.TestViewModels
{
    internal partial class CommonViewModel : ObservableObject ,IViewModelBase, INavigateAware
    {
        [ObservableProperty]
        private int _count = 0;

        public void NavigateTo(Parameters? parameters)
        {
            Count++;
        }
    }
}
