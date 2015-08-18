using System.Windows;
using System.Windows.Controls;
using Popcorn.Panels;
using Popcorn.Events;
using Popcorn.Helpers;
using Popcorn.ViewModels.Tabs;

namespace Popcorn.UserControls.Tabs
{
    /// <summary>
    /// Interaction logic for SearchMovies.xaml
    /// </summary>
    public partial class SearchMovies
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SearchMovies class.
        /// </summary>
        public SearchMovies()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Method -> ScrollViewerScrollChanged

        /// <summary>
        /// Decide if we have to load next page regarding to the scroll position
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">ScrollChangedEventArgs</param>
        private async void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var totalHeight = e.VerticalOffset + e.ViewportHeight;
            if (totalHeight.Equals(e.ExtentHeight))
            {
                var vm = DataContext as SearchTabViewModel;
                if (vm != null && !vm.IsLoadingMovies)
                {
                    await vm.SearchMoviesAsync(vm.SearchFilter);
                }
            }
        }

        #endregion

        #region Method -> ElasticWrapPanelLoaded

        /// <summary>
        /// Subscribe NumberOfColumnsChanged to the NumberOfColumnsChanged event of the ElasticWrapPanel
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void ElasticWrapPanelLoaded(object sender, RoutedEventArgs e)
        {
            var elasticWrapPanel = sender as ElasticWrapPanel;
            if (elasticWrapPanel != null)
            {
                elasticWrapPanel.NumberOfColumnsChanged += NumberOfColumnsChanged;
            }
        }

        #endregion

        #region Method -> NumberOfColumnsChanged

        /// <summary>
        /// When the column's number of the ElasticWrapPanel has changed, reset the MaxMoviesPerPage property to a value so that there's enough content to be able to scroll
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">NumberOfColumnChangedEventArgs</param>
        private void NumberOfColumnsChanged(object sender, NumberOfColumnChangedEventArgs e)
        {
            var vm = DataContext as TabsViewModel;
            if (vm != null)
            {
                vm.MaxMoviesPerPage = e.NumberOfColumns*Constants.NumberOfRowsPerPage;
            }
        }

        #endregion

        #endregion
    }
}