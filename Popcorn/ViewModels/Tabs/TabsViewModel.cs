using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Popcorn.Services.Movie;

namespace Popcorn.ViewModels.Tabs
{
    /// <summary>
    /// Manage tab controls
    /// </summary>
    public class TabsViewModel : ViewModelBase, ITab
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

        #region Command -> LikeMovieCommand

        /// <summary>
        /// Command used to like a movie
        /// </summary>
        public RelayCommand<MovieShort> LikeMovieCommand { get; private set; }

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
                    var tasks = new List<Task>();
                    int i = 0;
                    foreach (var movie in Movies)
                    {
                        i++;
                        var t =
                            Task.Delay(1000 * i)
                                .ContinueWith(async _ => { await MovieService.TranslateMovieShortAsync(movie); });
                        tasks.Add(t);
                    }
                    await Task.WhenAll(tasks);
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
            LikeMovieCommand =
                new RelayCommand<MovieShort>(async movie => { await UserService.LikeMovieAsync(movie); });
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