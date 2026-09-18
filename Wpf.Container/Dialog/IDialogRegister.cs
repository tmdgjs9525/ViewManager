using Container.Core.Interfaces;
using Container.Core.Parameter;
using System.Windows.Controls;

namespace Eye.Base.Dialog
{

    public interface IDialogRegister
    {
        public void AddTransientDialog<TView, TViewModel>() where TView : Control
                                                            where TViewModel : IViewModelBase, IDialogAware;
        public void AddSingletonDialog<TView, TViewModel>() where TView : Control
                                                            where TViewModel : IViewModelBase, IDialogAware;
    }

}
