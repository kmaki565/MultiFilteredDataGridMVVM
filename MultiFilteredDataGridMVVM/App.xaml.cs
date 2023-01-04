using Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using DataService;
using Microsoft.Extensions.DependencyInjection;
using MultiFilteredDataGridMVVM.Model;
using MultiFilteredDataGridMVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<IDataService, DummyService>()
                .AddTransient<MainViewModel>()
                .BuildServiceProvider());
        }
    }
}
