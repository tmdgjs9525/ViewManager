using Wpf.Container.Extensions;
using Wpf.Container.Test.TestViewModels;
using Wpf.Container.Test.TestViews;
using Eye.Base.Extensions;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Wpf.Container.Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost host;

        public App()
        {
            host = Host.CreateDefaultBuilder()
                       .ConfigureServices()
                       .Build();

            Ioc.Default.ConfigureServices(host.Services);
            host.RunAsync();
        }

        
    }

    public static class ViewerExtension
    {
        public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                // Services
                services.AddWpfContainerService();
                services.AddTransient<MainWindowViewModel>();
                services.AddTransientDialog<TestDialog,TestDialogViewModel>();

                var vm = new CommonViewModel();

                services.AddSingletonNavigation<BView, CommonViewModel>();

                var a = new CommonViewModel();
                services.AddTransientNavigation<AView>(a);

                //services.AddTransientNavigation<AView>(vm);
                //services.AddTransientNavigation<BView>(vm);
            });
        }
    }
}
