using Common;
using DataService;
using GalaSoft.MvvmLight;
using Unity;
using MultiFilteredDataGridMVVM.Model;
using Unity.Lifetime;

namespace MultiFilteredDataGridMVVM.ViewModel
{
    public class MainViewModelLocator
    {
        static MainViewModelLocator()
        {
            Container = new UnityContainer();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // if in design mode, use design data service
                Container.RegisterType<IDataService, DesignDummyService>();
            }
            else
            {
                // otherwise for runtime use real data source
                Container.RegisterType<IDataService, DummyService>();
            }

            // register as a singleton
            Container.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());
        }

        public static IUnityContainer Container
        {
            get;
            private set;
        }

        public MainViewModel MainVM
        {
            get
            {
                var vm = Container.Resolve<MainViewModel>();
                return vm;
            }
        }

        public static void Cleanup()
        {
            Container.Resolve<MainViewModel>().Cleanup();
        }
    }
}
