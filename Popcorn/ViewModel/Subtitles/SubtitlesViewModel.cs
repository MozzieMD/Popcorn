using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Popcorn.Model.Movie;
using System.Threading;
using System.Threading.Tasks;
using Popcorn.Service.Movie;

namespace Popcorn.ViewModel.Subtitles
{
    public class SubtitlesViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> MovieService

        /// <summary>
        /// The service used to interact with movies
        /// </summary>
        private IMovieService MovieService { get; }

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
        private CancellationTokenSource CancellationDownloadingSubtitlesToken { get; set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="movie">The movie</param>
        public SubtitlesViewModel(MovieFull movie)
        {
            MovieService = SimpleIoc.Default.GetInstance<IMovieService>();
            Movie = movie;
            CancellationDownloadingSubtitlesToken = new CancellationTokenSource();

            Task.Run(async () =>
            {
                await LoadSubtitlesAsync(Movie);
            });
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
            CancellationDownloadingSubtitlesToken?.Dispose();
        }

        #endregion

        public override void Cleanup()
        {
            StopDownloadingSubtitles();
            base.Cleanup();
        }
    }
}