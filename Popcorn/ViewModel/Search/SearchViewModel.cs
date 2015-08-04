using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;

namespace Popcorn.ViewModel.Search
{
    /// <summary>
    /// Movie's search
    /// </summary>
    public class SearchViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> SearchFilter

        /// <summary>
        /// The filter for searching movies
        /// </summary>
        private string _searchFilter;

        /// <summary>
        /// The filter for searching movies
        /// </summary>
        public string SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                Set(() => SearchFilter, ref _searchFilter, value, true);
            }
        }

        #endregion

        #endregion

        #region Commands

        #region Command -> SearchMovieCommand

        /// <summary>
        /// SearchMovieCommand
        /// </summary>
        public RelayCommand SearchMovieCommand { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// SearchViewModel
        /// </summary>
        public SearchViewModel()
        {
            // When search is empty, notify it
            Messenger.Default.Register<PropertyChangedMessage<string>>(this, e =>
            {
                if(e.PropertyName == GetPropertyName(() => SearchFilter) && string.IsNullOrEmpty(e.NewValue))
                {
                    Messenger.Default.Send(new SearchMovieMessage(string.Empty));
                }
            });

            // Search movies with the current SearchFilter as criteria
            SearchMovieCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new SearchMovieMessage(SearchFilter));
            });
        }
        #endregion

        #region Methods

        #endregion

        public override void Cleanup()
        {
            Messenger.Default.Unregister<PropertyChangedMessage<string>>(this);
            base.Cleanup();
        }
    }
}
