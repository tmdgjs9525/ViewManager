using Avalonia.Container.ServiceHelper;
using Avalonia.Container.Test.ViewModels;
using Avalonia.Container.Test.Views;
using Wpf.Container.Extensions;
using Eye.Base.Extensions;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Avalonia.Container.Test
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            var collection = new ServiceCollection();
            collection.AddAvaloniaContainerService().AddCommonServices();

            var services = collection.BuildServiceProvider();
            Ioc.Default.ConfigureServices(services);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var v = Ioc.Default.GetRequiredService<MainWindow>();
            var vm = Ioc.Default.GetRequiredService<MainWindowViewModel>();

            v.DataContext = vm;

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = v;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }

    }

    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddSingleton<MainWindow>();
            collection.AddSingleton<MainWindowViewModel>();

            collection.AddSingletonNavigation<MainView1, MainView1ViewModel>();
            collection.AddSingletonNavigation<MainView2, MainView2ViewModel>();

            collection.AddSingletonDialog<TestDialog, TestDialogViewModel>();
        }
    }
}