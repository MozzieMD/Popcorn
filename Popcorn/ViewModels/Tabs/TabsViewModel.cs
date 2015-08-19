using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Models.Movie;
using GalaSoft.MvvmLight.CommandWpf;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Services.User;
using GalaSoft.MvvmLight.Ioc;
using Popcorn.Services.History;
using Popcorn.Services.Movie;

namespace Popcorn.ViewModels.Tabs
{
    /// <summary>
    /// Manage tab controls
    /// </summary>
    public class TabsViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> MovieService

        /// <summary>
        /// Services used to interact with movies
        /// </summary>
        protected MovieService MovieService { get; }

        #endregion

        #region Property -> UserService

        /// <summary>
        /// Services used to interacts with user
        /// </summary>
        protected UserService UserService { get; }

        #endregion

        #region Property -> MovieHistoryService

        /// <summary>
        /// Services used to interacts with movie history
        /// </summary>
        protected MovieHistoryService MovieHistoryService { get; }

        #endregion

        #region Property -> Movies

        private ObservableCollection<MovieShort> _movies = new ObservableCollection<MovieShort>();

        /// <summary>
        /// Tab's movies
        /// </summary>
        public ObservableCollection<MovieShort> Movies
        {
            get { return _movies; }
            set { Set(() => Movies, ref _movies, value); }
        }

        #endregion

        #region Property -> CurrentNumberOfMovies

        private int _currentNumberofMovies;

        /// <summary>
        /// The current number of movies in the tab
        /// </summary>
        public int CurrentNumberOfMovies
        {
            get { return _currentNumberofMovies; }
            set { Set(() => CurrentNumberOfMovies, ref _currentNumberofMovies, value); }
        }

        #endregion

        #region Property -> MaxNumberOfMovies

        private int _maxNumberOfMovies;

        /// <summary>
        /// The maximum number of movies found
        /// </summary>
        public int MaxNumberOfMovies
        {
            get { return _maxNumberOfMovies; }
            set { Set(() => MaxNumberOfMovies, ref _maxNumberOfMovies, value); }
        }

        #endregion

        #region Property -> Page

        /// <summary>
        /// Current page number of loaded movies
        /// </summary>
        public int Page { get; protected set; }

        #endregion

        #region Property -> LastPage

        /// <summary>
        /// Last page number of loaded movies
        /// </summary>
        protected int LastPage { get; set; } = int.MaxValue;

        #endregion

        #region Property -> MaxMoviesPerPage

        /// <summary>
        /// Maximum movies number to load per page request
        /// </summary>
        public int MaxMoviesPerPage { protected get; set; }

        #endregion

        #region Property -> CancellationLoadNextPageToken

        /// <summary>
        /// Token to cancel movie loading
        /// </summary>
        protected CancellationTokenSource CancellationLoadNextPageToken { get; private set; }

        #endregion

        #region Property -> TabName

        private string _tabName;

        /// <summary>
        /// The name of the tab shown in the interface
        /// </summary>
        public string TabName
        {
            get { return _tabName; }
            set { Set(() => TabName, ref _tabName, value); }
        }

        #endregion

        #region Property -> IsLoadingMovies

        private bool _isLoadingMovies;

        /// <summary>
        /// Specify if movies are loading
        /// </summary>
        public bool IsLoadingMovies
        {
            get { return _isLoadingMovies; }
            protected set { Set(() => IsLoadingMovies, ref _isLoadingMovies, value); }
        }

        #endregion

        #region Property -> NumberOfLoadedMovies

        private bool _isMoviesFound = true;

        /// <summary>
        /// Indicates if there's any movie found
        /// </summary>
        public bool IsMovieFound
        {
            get { return _isMoviesFound; }
            set { Set(() => IsMovieFound, ref _isMoviesFound, value); }
        }

        #endregion

        #endregion

        #region Commands

        #region Command -> ReloadMovies

        /// <summary>
        /// Command used to reload movies
        /// </summary>
        public RelayCommand ReloadMovies { get; set; }

        #endregion

        #region Command -> SetFavoriteMovieCommand

        /// <summary>
        /// Command used to set a movie as favorite
        /// </summary>
        public RelayCommand<MovieShort> SetFavoriteMovieCommand { get; private set; }

        #endregion

        #endregion

        #region Constructors

        #region Constructor -> MoviesViewModel

        /// <summary>
        /// Initializes a new instance of the TabsViewModel class.
        /// </summary>
        protected TabsViewModel()
        {
            RegisterMessages();
            RegisterCommands();
            CancellationLoadNextPageToken = new CancellationTokenSource();
            MovieService = SimpleIoc.Default.GetInstance<MovieService>();
            UserService = SimpleIoc.Default.GetInstance<UserService>();
            MovieHistoryService = SimpleIoc.Default.GetInstance<MovieHistoryService>();
            MaxMoviesPerPage = Constants.MaxMoviesPerPage;
        }

        #endregion

        #endregion

        #region Methods

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<ChangeLanguageMessage>(
                this,
                async message =>
                {
                    foreach (var movie in Movies.ToList())
                    {
                        await Task.Delay(1000);
                        await MovieService.TranslateMovieShortAsync(movie);
                    }
                });

            Messenger.Default.Register<ChangeFavoriteMovieMessage>(
                this,
                async message =>
                {
                    await MovieHistoryService.ComputeMovieHistoryAsync(Movies);
                });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        /// <returns></returns>
        private void RegisterCommands()
        {
            SetFavoriteMovieCommand =
                new RelayCommand<MovieShort>(async movie =>
                {
                    await MovieHistoryService.SetFavoriteMovieAsync(movie);
                    Messenger.Default.Send(new ChangeFavoriteMovieMessage(movie));
                });
        }

        #endregion

        #region Method -> LoadMoviesAsync

        /// <summary>
        /// Load movies
        /// </summary>
        public virtual Task LoadMoviesAsync()
        {
            return null;
        }
        #endregion

        #region Method -> StopLoadingNextPage

        /// <summary>
        /// Cancel the loading of the next page 
        /// </summary>
        private void StopLoadingNextPage()
        {
            CancellationLoadNextPageToken?.Cancel();
            CancellationLoadNextPageToken = new CancellationTokenSource();
        }

        #endregion

        public override void Cleanup()
        {
            StopLoadingNextPage();
            CancellationLoadNextPageToken?.Dispose();

            base.Cleanup();
        }

        #endregion
    }
}