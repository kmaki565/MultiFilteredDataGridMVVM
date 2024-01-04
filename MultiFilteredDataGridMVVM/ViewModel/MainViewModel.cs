//using GalaSoft.MvvmLight;
using Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MultiFilteredDataGridMVVM.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace MultiFilteredDataGridMVVM.ViewModel
{
    public class MainViewModel : ObservableRecipient
    {
        #region Members

        private string _selectedAuthor = null;
        private string _selectedCountry = null;
        private Nullable<int> _selectedYear = null;
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
            Messenger.Register<MainViewModel, ViewCollectionViewSourceMessageToken>(this, (r, m) => r.Handle_ViewCollectionViewSourceMessageToken(m));
        }

        /// <summary>
        /// Gets or sets the IDownloadDataService member
        /// </summary>
        internal IDataService DataService { get; }
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
                OnPropertyChanged();
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
                OnPropertyChanged();
                ApplyAuthorFilter();
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
                OnPropertyChanged();
                ApplyCountryFilter();
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
                OnPropertyChanged();
                ApplyYearFilter();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
                // Apparently we need this to convey the change to the button UI with the new MVVM lib.
                RemoveCountryFilterCommand.NotifyCanExecuteChanged();
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
                OnPropertyChanged();
                RemoveAuthorFilterCommand.NotifyCanExecuteChanged();
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
                OnPropertyChanged();
                RemoveYearFilterCommand.NotifyCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand ResetFiltersCommand
        {
            get;
            private set;
        }
        public RelayCommand RemoveCountryFilterCommand
        {
            get;
            private set;
        }
        public RelayCommand RemoveAuthorFilterCommand
        {
            get;
            private set;
        }
        public RelayCommand RemoveYearFilterCommand
        {
            get;
            private set;
        }

        #endregion

        private void InitializeCommands()
        {
            ResetFiltersCommand = new RelayCommand(ResetFilters);
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
            CVS.Filter += new FilterEventHandler(FilterByCountry);
            CVS.Filter += new FilterEventHandler(FilterByAuthor);
            CVS.Filter += new FilterEventHandler(FilterByYear);
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
            SelectedCountry = null;
        }
        public void RemoveAuthorFilter()
        {
            SelectedAuthor = null;
        }
        public void RemoveYearFilter()
        {
            SelectedYear = null;
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

        public void ApplyCountryFilter()
        {
            CanRemoveCountryFilter = !string.IsNullOrEmpty(_selectedCountry);
            CVS.View.Refresh();
        }
        public void ApplyAuthorFilter()
        {
            CanRemoveAuthorFilter = !string.IsNullOrEmpty(_selectedAuthor);
            CVS.View.Refresh();
        }
        public void ApplyYearFilter()
        {
            CanRemoveYearFilter = _selectedYear.HasValue;
            CVS.View.Refresh();
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
            if (!string.IsNullOrEmpty(_selectedAuthor))
            {
                if (e.Item is not Thing src)
                    e.Accepted = false;
                else if (_selectedAuthor != src.Author)
                    e.Accepted = false;
            }
        }
        private void FilterByYear(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            if (_selectedYear.HasValue)
            {
                if (e.Item is not Thing src)
                    e.Accepted = false;
                else if (_selectedYear != src.Year)
                    e.Accepted = false;
            }
        }
        private void FilterByCountry(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            if (!string.IsNullOrEmpty(_selectedCountry))
            {
                if (e.Item is not Thing src)
                    e.Accepted = false;
                else if (_selectedCountry != src.Country)
                    e.Accepted = false;
            }
        }
    }
}
