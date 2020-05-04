//using GalaSoft.MvvmLight;

//namespace MultiFilteredDataGridMVVM.ViewModel
//{
//    /// <summary>
//    /// This class contains properties that the main View can data bind to.
//    /// <para>
//    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
//    /// </para>
//    /// <para>
//    /// You can also use Blend to data bind with the tool's support.
//    /// </para>
//    /// <para>
//    /// See http://www.galasoft.ch/mvvm
//    /// </para>
//    /// </summary>
//    public class MainViewModel : ViewModelBase
//    {
//        /// <summary>
//        /// Initializes a new instance of the MainViewModel class.
//        /// </summary>
//        public MainViewModel()
//        {
//            ////if (IsInDesignMode)
//            ////{
//            ////    // Code runs in Blend --> create design time data.
//            ////}
//            ////else
//            ////{
//            ////    // Code runs "for real"
//            ////}
//        }
//    }
//}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MultiFilteredDataGridMVVM.Helpers;

namespace MultiFilteredDataGridMVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Members

        private string _selectedAuthor;
        private string _selectedCountry;
        private Nullable<int> _selectedYear;
        private ObservableCollection<string> _authors;
        private ObservableCollection<string> _countries;
        private ObservableCollection<int> _years;
        ObservableCollection<Thing> _things;
        private bool _canCanRemoveCountryFilter;
        private bool _canCanRemoveAuthorFilter;
        private bool _canCanRemoveYearFilter;

        #endregion

        public MainViewModel(IDataService dataService)
        {
            InitializeCommands();
            DataService = dataService;
            LoadData();

            //--------------------------------------------------------------
            // This 'registers' the instance of this view model to recieve messages with this type of token.  This 
            // is used to recieve a reference from the view that the collectionViewSource has been instantiated
            // and to recieve a reference to the CollectionViewSource which will be used in the view model for 
            // filtering
            Messenger.Default.Register<ViewCollectionViewSourceMessageToken>(this, Handle_ViewCollectionViewSourceMessageToken);
        }
        public override void Cleanup()
        {
            Messenger.Default.Unregister<ViewCollectionViewSourceMessageToken>(this);
            base.Cleanup();
        }

        /// <summary>
        /// Gets or sets the IDownloadDataService member
        /// </summary>
        internal IDataService DataService { get; set; }
        /// <summary>
        /// Gets or sets the CollectionViewSource which is the proxy for the 
        /// collection of Things and the datagrid in which each thing is displayed.
        /// </summary>
        private CollectionViewSource CVS { get; set; }

        #region Properties (Displayable in View)

        /// <summary>
        /// Gets or sets the primary collection of Thing objects to be displayed
        /// </summary>
        public ObservableCollection<Thing> Things
        {
            get { return _things; }
            set
            {
                if (_things == value)
                    return;
                _things = value;
                RaisePropertyChanged("Things");
            }
        }

        // Filter properties =============

        /// <summary>
        /// Gets or sets the selected author in the list authors to filter the collection
        /// </summary>
        public string SelectedAuthor
        {
            get { return _selectedAuthor; }
            set
            {
                if (_selectedAuthor == value)
                    return;
                _selectedAuthor = value;
                RaisePropertyChanged("SelectedAuthor");
                ApplyFilter(!string.IsNullOrEmpty(_selectedAuthor) ? FilterField.Author : FilterField.None);
            }
        }
        /// <summary>
        /// Gets or sets the selected author in the list countries to filter the collection
        /// </summary>
        public string SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                if (_selectedCountry == value)
                    return;
                _selectedCountry = value;
                RaisePropertyChanged("SelectedCountry");
                ApplyFilter(!string.IsNullOrEmpty(_selectedCountry) ? FilterField.Country : FilterField.None);
            }
        }
        /// <summary>
        /// Gets or sets the selected author in the list years to filter the collection
        /// </summary>
        public Nullable<int> SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                if (_selectedYear == value)
                    return;
                _selectedYear = value;
                RaisePropertyChanged("SelectedYear");
                ApplyFilter(_selectedYear.HasValue ? FilterField.Year : FilterField.None);
            }
        }

        /// <summary>
        /// Gets or sets a list of authors which is used to populate the author filter
        /// drop down list.
        /// </summary>
        public ObservableCollection<string> Authors
        {
            get { return _authors; }
            set
            {
                if (_authors == value)
                    return;
                _authors = value;
                RaisePropertyChanged("Authors");
            }
        }
        /// <summary>
        /// Gets or sets a list of authors which is used to populate the country filter
        /// drop down list.
        /// </summary>
        public ObservableCollection<string> Countries
        {
            get { return _countries; }
            set
            {
                if (_countries == value)
                    return;
                _countries = value;
                RaisePropertyChanged("Countries");
            }
        }
        /// <summary>
        /// Gets or sets a list of authors which is used to populate the year filter
        /// drop down list.
        /// </summary>
        public ObservableCollection<int> Years
        {
            get { return _years; }
            set
            {
                if (_years == value)
                    return;
                _years = value;
                RaisePropertyChanged("Years");
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating if the Country filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveCountryFilter
        {
            get { return _canCanRemoveCountryFilter; }
            set
            {
                _canCanRemoveCountryFilter = value;
                RaisePropertyChanged("CanRemoveCountryFilter");
            }
        }
        /// <summary>
        /// Gets or sets a flag indicating if the Author filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveAuthorFilter
        {
            get { return _canCanRemoveAuthorFilter; }
            set
            {
                _canCanRemoveAuthorFilter = value;
                RaisePropertyChanged("CanRemoveAuthorFilter");
            }
        }
        /// <summary>
        /// Gets or sets a flag indicating if the Year filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveYearFilter
        {
            get { return _canCanRemoveYearFilter; }
            set
            {
                _canCanRemoveYearFilter = value;
                RaisePropertyChanged("CanRemoveYearFilter");
            }
        }

        #endregion

        #region Commands

        public ICommand ResetFiltersCommand
        {
            get;
            private set;
        }
        public ICommand RemoveCountryFilterCommand
        {
            get;
            private set;
        }
        public ICommand RemoveAuthorFilterCommand
        {
            get;
            private set;
        }
        public ICommand RemoveYearFilterCommand
        {
            get;
            private set;
        }

        #endregion

        private void InitializeCommands()
        {
            ResetFiltersCommand = new RelayCommand(ResetFilters, null);
            RemoveCountryFilterCommand = new RelayCommand(RemoveCountryFilter, () => CanRemoveCountryFilter);
            RemoveAuthorFilterCommand = new RelayCommand(RemoveAuthorFilter, () => CanRemoveAuthorFilter);
            RemoveYearFilterCommand = new RelayCommand(RemoveYearFilter, () => CanRemoveYearFilter);
        }
        private void LoadData()
        {
            var things = DataService.GetThings();
            var q1 = from t in things
                     select t.Author;
            Authors = new ObservableCollection<string>(q1.Distinct());

            var q2 = from t in things
                     select t.Country;
            Countries = new ObservableCollection<string>(q2.Distinct());

            var q3 = from t in things
                     select t.Year;
            Years = new ObservableCollection<int>(q3.Distinct());

            Things = new ObservableCollection<Thing>(things);
        }
        /// <summary>
        /// This method handles a message recieved from the View which enables a reference to the
        /// instantiated CollectionViewSource to be used in the ViewModel.
        /// </summary>
        private void Handle_ViewCollectionViewSourceMessageToken(ViewCollectionViewSourceMessageToken token)
        {
            CVS = token.CVS;
        }

        // Command methods (called by the commands) ===============

        public void ResetFilters()
        {
            // clear filters 
            RemoveYearFilter();
            RemoveAuthorFilter();
            RemoveCountryFilter();
        }
        public void RemoveCountryFilter()
        {
            CVS.Filter -= new FilterEventHandler(FilterByCountry);
            SelectedCountry = null;
            CanRemoveCountryFilter = false;
        }
        public void RemoveAuthorFilter()
        {
            CVS.Filter -= new FilterEventHandler(FilterByAuthor);
            SelectedAuthor = null;
            CanRemoveAuthorFilter = false;
        }
        public void RemoveYearFilter()
        {
            CVS.Filter -= new FilterEventHandler(FilterByYear);
            SelectedYear = null;
            CanRemoveYearFilter = false;
        }

        // Other helper methods ==============

        /* Notes on Adding Filters:
         *   Each filter is added by subscribing a filter method to the Filter event
         *   of the CVS.  Filters are applied in the order in which they were added. 
         *   To prevent adding filters mulitple times ( because we are using drop down lists
         *   in the view), the CanRemove***Filter flags are used to ensure each filter
         *   is added only once.  If a filter has been added, its corresponding CanRemove***Filter
         *   is set to true.       
         *   
         *   If a filter has been applied already (for example someone selects "Canada" to filter by country
         *   and then they change their selection to another value (say "Mexico") we need to undo the previous
         *   country filter then apply the new one.  This does not completey Reset the filter, just
         *   allows it to be changed to another filter value. This applies to the other filters as well
         */

        public void AddCountryFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveCountryFilter)
            {
                CVS.Filter -= new FilterEventHandler(FilterByCountry);
                CVS.Filter += new FilterEventHandler(FilterByCountry);
            }
            else
            {
                CVS.Filter += new FilterEventHandler(FilterByCountry);
                CanRemoveCountryFilter = true;
            }
        }
        public void AddAuthorFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveAuthorFilter)
            {
                CVS.Filter -= new FilterEventHandler(FilterByAuthor);
                CVS.Filter += new FilterEventHandler(FilterByAuthor);
            }
            else
            {
                CVS.Filter += new FilterEventHandler(FilterByAuthor);
                CanRemoveAuthorFilter = true;
            }
        }
        public void AddYearFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveYearFilter)
            {
                CVS.Filter -= new FilterEventHandler(FilterByYear);
                CVS.Filter += new FilterEventHandler(FilterByYear);
            }
            else
            {
                CVS.Filter += new FilterEventHandler(FilterByYear);
                CanRemoveYearFilter = true;
            }
        }

        /* Notes on Filter Methods:
         * When using multiple filters, do not explicitly set anything to true.  Rather,
         * only hide things which do not match the filter criteria
         * by setting e.Accepted = false.  If you set e.Accept = true, if effectively
         * clears out any previous filters applied to it.  
         */

        private void FilterByAuthor(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as Thing;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedAuthor, src.Author) != 0)
                e.Accepted = false;
        }
        private void FilterByYear(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as Thing;
            if (src == null)
                e.Accepted = false;
            else if (SelectedYear != src.Year)
                e.Accepted = false;
        }
        private void FilterByCountry(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as Thing;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedCountry, src.Country) != 0)
                e.Accepted = false;
        }

        private enum FilterField
        {
            Author,
            Country,
            Year,
            None
        }
        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.Author:
                    AddAuthorFilter();
                    break;
                case FilterField.Country:
                    AddCountryFilter();
                    break;
                case FilterField.Year:
                    AddYearFilter();
                    break;
                default:
                    break;
            }
        }
    }
}
