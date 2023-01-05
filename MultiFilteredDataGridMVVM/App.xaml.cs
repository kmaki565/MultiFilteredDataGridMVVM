using Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using DataService;
using Microsoft.Extensions.DependencyInjection;
using MultiFilteredDataGridMVVM.ViewModel;
using System.Windows;

namespace MultiFilteredDataGridMVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //TODO: Show data from DesignDummyService in the XAML Designer.
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<IDataService, DummyService>()
                .AddTransient<MainViewModel>()
                .BuildServiceProvider());
        }
    }
}
