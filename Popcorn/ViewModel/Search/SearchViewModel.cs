using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;

namespace Popcorn.ViewModel.Search
{
    /// <summary>
    /// Movie's search
    /// </summary>
    public sealed class SearchViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> SearchFilter

        private string _searchFilter;

        /// <summary>
        /// The filter for searching movies
        /// </summary>
        public string SearchFilter
        {
            get { return _searchFilter; }
            set { Set(() => SearchFilter, ref _searchFilter, value, true); }
        }

        #endregion

        #endregion

        #region Commands

        #region Command -> SearchMovieCommand

        /// <summary>
        /// Command used to search movies
        /// </summary>
        public RelayCommand SearchMovieCommand { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SearchViewModel class.
        /// </summary>
        private SearchViewModel()
        {
            RegisterMessages();
            RegisterCommands();
        }

        #endregion

        #region Methods

        #region Method -> CreateInstance

        /// <summary>
        /// Initialize an instance of the SearchViewModel class
        /// </summary>
        /// <returns>Instance of SearchViewModel</returns>
        public static SearchViewModel CreateInstance()
        {
            return new SearchViewModel();
        }

        #endregion

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<PropertyChangedMessage<string>>(this, e =>
            {
                if (e.PropertyName == GetPropertyName(() => SearchFilter) && string.IsNullOrEmpty(e.NewValue))
                {
                    Messenger.Default.Send(new SearchMovieMessage(string.Empty));
                }
            });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            SearchMovieCommand =
                new RelayCommand(() => { Messenger.Default.Send(new SearchMovieMessage(SearchFilter)); });
        }

        #endregion

        #endregion
    }
}