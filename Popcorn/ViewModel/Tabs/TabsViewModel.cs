using System.Collections.ObjectModel;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Service.Api;
using Popcorn.Model.Movie;
using GalaSoft.MvvmLight.CommandWpf;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Service.User;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;

namespace Popcorn.ViewModel.Tabs
{
    /// <summary>
    /// The movies' content
    /// </summary>
    public class TabsViewModel : ViewModelBase, ITab
    {
        #region Properties

        #region Property -> ApiService

        /// <summary>
        /// Service used to consume the API
        /// </summary>
        protected IApiService ApiService { get; }

        #endregion

        #region Property -> LanguageService
        /// <summary>
        /// Service used to interacts with user data
        /// </summary>
        protected IUserDataService UserDataService { get; }
        #endregion

        #region Property -> Movies

        /// <summary>
        /// Movies loaded from the service and shown in the interface
        /// </summary>
        private ObservableCollection<MovieShort> _movies = new ObservableCollection<MovieShort>();

        public ObservableCollection<MovieShort> Movies
        {
            get { return _movies; }
            set { Set(() => Movies, ref _movies, value); }
        }

        #endregion

        #region Property -> Page

        /// <summary>
        /// Current page number of loaded movies
        /// </summary>
        protected int Page { get; set; }

        #endregion

        #region Property -> MaxMoviesPerPage

        /// <summary>
        /// Maximum movies number to load per page request
        /// </summary>
        public int MaxMoviesPerPage { get; set; }

        #endregion

        #region Property -> CancellationLoadNextPageToken

        /// <summary>
        /// Token to cancel movie loading
        /// </summary>
        protected CancellationTokenSource CancellationLoadNextPageToken { get; set; }
        #endregion

        #region Property -> TabName

        /// <summary>
        /// The name of the tab shown in the interface
        /// </summary>
        private string _tabName;

        public string TabName
        {
            get
            {
                return _tabName;
            }
            set
            {
                Set(() => TabName, ref _tabName, value);
            }
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

        #region Property -> IsConnectionInError

        private bool _isConnectionInError;

        /// <summary>
        /// Specify if a connection error has occured
        /// </summary>
        public bool IsConnectionInError
        {
            get { return _isConnectionInError; }
            set { Set(() => IsConnectionInError, ref _isConnectionInError, value); }
        }

        #endregion

        #endregion

        #region Commands

        #region Command -> ReloadMovies

        /// <summary>
        /// Reload movies 
        /// </summary>
        public RelayCommand ReloadMovies { get; set; }

        #endregion

        #region Command -> LikeMovieCommand

        /// <summary>
        /// Like a movie
        /// </summary>
        public RelayCommand<MovieShort> LikeMovieCommand { get; private set; }

        #endregion

        #endregion

        #region Constructors

        #region Constructor -> MoviesViewModel

        /// <summary>
        /// Initializes a new instance of the MoviesViewModel class.
        /// </summary>
        protected TabsViewModel()
        {
            // Set the CancellationToken for having the possibility to stop loading movies
            CancellationLoadNextPageToken = new CancellationTokenSource();

            ApiService = SimpleIoc.Default.GetInstance<IApiService>();
            UserDataService = SimpleIoc.Default.GetInstance<IUserDataService>();

            MaxMoviesPerPage = Constants.MaxMoviesPerPage;

            // A connection error occured
            Messenger.Default.Register<ConnectionErrorMessage>(this, e =>
            {
                if (!e.ResetConnectionError)
                {
                    IsConnectionInError = true;
                }
                else
                {
                    IsConnectionInError = !e.ResetConnectionError;
                }
            });

            Messenger.Default.Register<ChangeLanguageMessage>(
            this,
            async message =>
            {
                var tasks = new List<Task>();
                int i = 0;
                foreach (var movie in Movies)
                {
                    i++;
                    var t = Task.Delay(1000 * i).ContinueWith(_ =>
                    {
                        ApiService.TranslateMovieShortAsync(movie);
                    });
                    tasks.Add(t);
                }
                await Task.WhenAll(tasks);
            });

            // Record the like action to the database
            LikeMovieCommand = new RelayCommand<MovieShort>(async movie =>
            {
                await (new UserDataService()).LikeMovieAsync(movie);
            });
        }

        #endregion

        #endregion

        #region Methods

        #region Method -> StopLoadingNextPage

        /// <summary>
        /// Cancel the loading of the next page 
        /// </summary>
        protected void StopLoadingNextPage()
        {
            CancellationLoadNextPageToken?.Cancel(true);
            CancellationLoadNextPageToken?.Dispose();
            CancellationLoadNextPageToken = new CancellationTokenSource();
        }

        #endregion

        #endregion

        public override void Cleanup()
        {
            Messenger.Default.Unregister<ConnectionErrorMessage>(this);
            Messenger.Default.Unregister<ChangeLanguageMessage>(this);

            base.Cleanup();
        }
    }
}
