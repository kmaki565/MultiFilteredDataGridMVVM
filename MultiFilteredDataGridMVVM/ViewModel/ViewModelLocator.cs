/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:MultiFilteredDataGridMVVM"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using DataService;
using Microsoft.Extensions.DependencyInjection;

namespace MultiFilteredDataGridMVVM.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //if (DesignerProperties.GetIsInDesignMode(MainVM))
            //{
            //    // Create design time view services and models
            //    SimpleIoc.Default.Register<IDataService, DesignDummyService>();
            //}
            //else
            //{
            //    // Create run time view services and models
            //    SimpleIoc.Default.Register<IDataService, DummyService>();
            //}

            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<IDataService, DummyService>()
                .AddTransient<MainViewModel>()
                .BuildServiceProvider());
        }

        public MainViewModel MainVM
        {
            get
            {
                return Ioc.Default.GetRequiredService<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            //ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
        }
    }
}