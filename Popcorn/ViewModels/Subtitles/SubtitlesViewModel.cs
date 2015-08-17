using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Popcorn.Models.Movie;
using System.Threading;
using System.Threading.Tasks;
using Popcorn.Services.Movie;

namespace Popcorn.ViewModels.Subtitles
{
    public sealed class SubtitlesViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> MovieService

        /// <summary>
        /// The service used to interact with movies
        /// </summary>
        private MovieService MovieService { get; }

        #endregion

        #region Property -> Movie

        private MovieFull _movie;

        /// <summary>
        /// The movie
        /// </summary>
        public MovieFull Movie
        {
            get { return _movie; }
            set { Set(() => Movie, ref _movie, value); }
        }

        #endregion

        #region Property -> CancellationDownloadingSubtitlesToken

        /// <summary>
        /// Token to cancel downloading subtitles
        /// </summary>
        private CancellationTokenSource CancellationDownloadingSubtitlesToken { get; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SubtitlesViewModel class.
        /// </summary>
        /// <param name="movie">The movie</param>
        private SubtitlesViewModel(MovieFull movie)
        {
            CancellationDownloadingSubtitlesToken = new CancellationTokenSource();
            MovieService = SimpleIoc.Default.GetInstance<MovieService>();
            Movie = movie;
        }

        #endregion

        #region Methods

        #region Method -> InitializeAsync
        /// <summary>
        /// Load asynchronously the movie's subtitles for the current instance
        /// </summary>
        /// <returns>Instance of SubtitlesViewModel</returns>
        private async Task<SubtitlesViewModel> InitializeAsync()
        {
            await LoadSubtitlesAsync(Movie);
            return this;
        }
        #endregion

        #region Method -> CreateAsync
        /// <summary>
        /// Initialize asynchronously an instance of the SubtitlesViewModel class
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <returns>Instance of SubtitlesViewModel</returns>
        public static Task<SubtitlesViewModel> CreateAsync(MovieFull movie)
        {
            var ret = new SubtitlesViewModel(movie);
            return ret.InitializeAsync();
        }
        #endregion

        #region Method -> LoadSubtitlesAsync

        /// <summary>
        /// Get the movie's subtitles
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <returns></returns>
        private async Task LoadSubtitlesAsync(MovieFull movie)
        {
            await MovieService.LoadSubtitlesAsync(movie, CancellationDownloadingSubtitlesToken.Token);
        }

        #endregion

        #region Method -> StopDownloadingSubtitles

        /// <summary>
        /// Stop downloading subtitles
        /// </summary>
        private void StopDownloadingSubtitles()
        {
            CancellationDownloadingSubtitlesToken?.Cancel();
        }

        #endregion

        public override void Cleanup()
        {
            StopDownloadingSubtitles();
            CancellationDownloadingSubtitlesToken?.Dispose();
            base.Cleanup();
        }

        #endregion
    }
}