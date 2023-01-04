﻿using Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using DataService;
using Microsoft.Extensions.DependencyInjection;
using MultiFilteredDataGridMVVM.Helpers;
using MultiFilteredDataGridMVVM.Model;
using MultiFilteredDataGridMVVM.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace MultiFilteredDataGridMVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            //if (DesignerProperties.GetIsInDesignMode(this))
            //    Ioc.Default.ConfigureServices(
            //        new ServiceCollection()
            //        .AddSingleton<IDataService, DesignDummyService>()
            //        .AddTransient<MainViewModel>()
            //        .BuildServiceProvider());
            //else 
            //    Ioc.Default.ConfigureServices(
            //        new ServiceCollection()
            //        .AddSingleton<IDataService, DummyService>()
            //        .AddTransient<MainViewModel>()
            //        .BuildServiceProvider());

            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainViewModel>();

            // Here we send a message which is caught by the view model.  The message contains a reference
            // to the CollectionViewSource which is instantiated when the view is instantiated (before the view model).
            WeakReferenceMessenger.Default.Send(new ViewCollectionViewSourceMessageToken() { CVS = (CollectionViewSource)(this.Resources["X_CVS"]) });

            // Note to MVVM purists:  Not an ideal solution.  But based on the amount if time spent on this it was acceptable, especially to the client.
        }
    }
}
