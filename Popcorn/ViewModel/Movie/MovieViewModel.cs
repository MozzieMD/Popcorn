using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using Popcorn.Service.Api;
using Popcorn.ViewModel.Download;
using Popcorn.ViewModel.Trailer;

namespace Popcorn.ViewModel.Movie
{
    /// <summary>
    /// Movie page
    /// </summary>
    public class MovieViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> ApiService

        /// <summary>
        /// The service used to consume APIs
        /// </summary>
        private IApiService ApiService { get; }

        #endregion

        #region Property -> Movie

        /// <summary>
        /// The page's movie
        /// </summary>
        private MovieFull _movie = new MovieFull();

        public MovieFull Movie
        {
            get { return _movie; }
            set { Set(() => Movie, ref _movie, value); }
        }

        #endregion

        #region Property -> IsMovieLoading

        /// <summary>
        /// Indicates if a movie is loading
        /// </summary>
        private bool _isMovieLoading;

        public bool IsMovieLoading
        {
            get { return _isMovieLoading; }
            set { Set(() => IsMovieLoading, ref _isMovieLoading, value); }
        }

        #endregion

        #region Property -> DownloadMovie

        /// <summary>
        /// View model which takes care of downloading the movie
        /// </summary>
        private DownloadMovieViewModel _downloadMovie;

        public DownloadMovieViewModel DownloadMovie
        {
            get { return _downloadMovie; }
            set { Set(() => DownloadMovie, ref _downloadMovie, value); }
        }

        #endregion

        #region Property -> Trailer

        /// <summary>
        /// View model which takes care of the movie's trailer
        /// </summary>
        private TrailerViewModel _trailer;

        public TrailerViewModel Trailer
        {
            get { return _trailer; }
            set { Set(() => Trailer, ref _trailer, value); }
        }

        #endregion

        #region Property -> IsPlayingTrailer

        /// <summary>
        /// Specify if a trailer is loading
        /// </summary>
        private bool _isPlayingTrailer;

        /// <summary>
        /// Specify if a trailer is loading
        /// </summary>
        public bool IsPlayingTrailer
        {
            get { return _isPlayingTrailer; }
            set { Set(() => IsPlayingTrailer, ref _isPlayingTrailer, value); }
        }

        #endregion

        #region Property -> IsDownloadingMovie

        /// <summary>
        /// Specify if a movie is downloading
        /// </summary>
        private bool _isDownloadingMovie;

        /// <summary>
        /// Specify if a movie is downloading
        /// </summary>
        public bool IsDownloadingMovie
        {
            get { return _isDownloadingMovie; }
            set { Set(() => IsDownloadingMovie, ref _isDownloadingMovie, value); }
        }

        #endregion

        #region Property -> CancellationLoadingToken

        /// <summary>
        /// Token to cancel movie loading
        /// </summary>
        private CancellationTokenSource CancellationLoadingToken { get; set; }

        #endregion

        #endregion

        #region Commands

        #region Command -> LoadMovieCommand

        /// <summary>
        /// LoadMovieCommand
        /// </summary>
        public RelayCommand<MovieShort> LoadMovieCommand { get; private set; }

        #endregion

        #region Command -> PlayMovieCommand

        /// <summary>
        /// PlayMovieCommand
        /// </summary>
        public RelayCommand PlayMovieCommand { get; private set; }

        #endregion

        #region Command -> PlayTrailerCommand

        /// <summary>
        /// PlayTrailerCommand
        /// </summary>
        public RelayCommand PlayTrailerCommand { get; private set; }

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public MovieViewModel()
        {
            ApiService = SimpleIoc.Default.GetInstance<IApiService>();

            // Set the CancellationToken for having the possibility to stop loading a movie
            CancellationLoadingToken = new CancellationTokenSource();

            // Stop playing trailer
            Messenger.Default.Register<StopPlayingTrailerMessage>(
            this,
            message =>
            {
                StopPlayingTrailer();
            });

            // Stop playing movie
            Messenger.Default.Register<StopPlayingMovieMessage>(
            this,
            message =>
            {
                StopPlayingMovie();
            });

            Messenger.Default.Register<ChangeLanguageMessage>(
            this,
            async message =>
            {
                if (!string.IsNullOrEmpty(Movie?.ImdbCode))
                {
                    await ApiService.TranslateMovieFullAsync(Movie);
                }
            });

            // Load requested movie
            LoadMovieCommand = new RelayCommand<MovieShort>(async movie =>
            {
                await LoadMovieAsync(movie);
            });

            // Download the requested movie
            PlayMovieCommand = new RelayCommand(() =>
            {
                IsDownloadingMovie = true;
                DownloadMovie = new DownloadMovieViewModel(Movie);
            });

            // Load requested movie trailer
            PlayTrailerCommand = new RelayCommand(() =>
            {
                IsPlayingTrailer = true;
                Trailer = new TrailerViewModel(Movie);
            });
        }

        #endregion

        #region Method -> LoadMovieAsync

        /// <summary>
        /// Get the requested movie
        /// </summary>
        /// <param name="movieToLoad">Movie</param>
        private async Task LoadMovieAsync(MovieShort movieToLoad)
        {
            Messenger.Default.Send(new LoadMovieMessage(movieToLoad));
            IsMovieLoading = true;
            try
            {
                Movie = await ApiService.GetMovieFullDetailsAsync(movieToLoad).ConfigureAwait(false);
                IsMovieLoading = false;

                // Download the movie poster
                Movie.PosterImagePath =
                    await
                        ApiService.DownloadPosterImageAsync(Movie.ImdbCode,
                                new Uri(Movie.Images.LargeCoverImage),
                                CancellationLoadingToken.Token).ConfigureAwait(false);

                // For each director, we download its image
                foreach (var director in Movie.Directors)
                {
                    director.SmallImagePath =
                        await
                            ApiService.DownloadDirectorImageAsync(director.Name,
                                new Uri(director.SmallImage),
                                CancellationLoadingToken.Token).ConfigureAwait(false);
                }

                // For each actor, we download its image
                foreach (var actor in Movie.Actors)
                {
                    actor.SmallImagePath =
                        await
                            ApiService.DownloadActorImageAsync(actor.Name,
                                new Uri(actor.SmallImage),
                                CancellationLoadingToken.Token).ConfigureAwait(false);
                }

                Movie.BackgroundImagePath =
                    await
                        ApiService.DownloadBackgroundImageAsync(Movie.ImdbCode,
                                CancellationLoadingToken.Token).ConfigureAwait(false);
            }
            catch (ApiServiceException e)
            {
                IsMovieLoading = false;
                if (e.Status == ApiServiceException.State.ConnectionError)
                {
                    Messenger.Default.Send(new ConnectionErrorMessage(e.Message));
                }
            }
        }

        #endregion

        #region Method -> StopLoadingMovie

        /// <summary>
        /// Stop loading the movie
        /// </summary>
        private void StopLoadingMovie()
        {
            IsMovieLoading = false;

            CancellationLoadingToken?.Cancel();
            CancellationLoadingToken?.Dispose();
            CancellationLoadingToken = new CancellationTokenSource();
        }

        #endregion

        #region Method -> StopPlayingTrailer

        /// <summary>
        /// Stop playing a trailer
        /// </summary>
        private void StopPlayingTrailer()
        {
            IsPlayingTrailer = false;
            Trailer = null;
        }

        #endregion

        #region Method -> StopPlayingMovie

        /// <summary>
        /// Stop playing a movie
        /// </summary>
        private void StopPlayingMovie()
        {
            IsDownloadingMovie = false;

            DownloadMovie?.Cleanup();
            DownloadMovie = null;
        }

        #endregion

        public override void Cleanup()
        {
            StopLoadingMovie();
            StopPlayingTrailer();
            StopPlayingMovie();
            base.Cleanup();
        }
    }
}
