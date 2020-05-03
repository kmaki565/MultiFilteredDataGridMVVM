using System.Windows.Data;

namespace MultiFilteredDataGridMVVM.Helpers
{
    /// <summary>
    /// This is a simple data class used to transport a reference to the CollectionViewSource
    /// from the view to the view model.
    /// </summary>
    public class ViewCollectionViewSourceMessageToken
    {
        public CollectionViewSource CVS { get; set; }
    }
}