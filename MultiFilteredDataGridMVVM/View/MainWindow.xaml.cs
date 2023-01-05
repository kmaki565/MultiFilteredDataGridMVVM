using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using MultiFilteredDataGridMVVM.Helpers;
using MultiFilteredDataGridMVVM.ViewModel;
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
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainViewModel>();

            // Here we send a message which is caught by the view model.  The message contains a reference
            // to the CollectionViewSource which is instantiated when the view is instantiated (before the view model).
            WeakReferenceMessenger.Default.Send(new ViewCollectionViewSourceMessageToken() { CVS = (CollectionViewSource)(this.Resources["X_CVS"]) });

            // Note to MVVM purists:  Not an ideal solution.  But based on the amount if time spent on this it was acceptable, especially to the client.
        }
    }
}
