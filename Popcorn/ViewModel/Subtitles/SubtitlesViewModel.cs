using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Popcorn.Model.Movie;
using Popcorn.Service.Api;
using System.Threading;
using System.Threading.Tasks;

namespace Popcorn.ViewModel.Subtitles
{
    public class SubtitlesViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> ApiService

        /// <summary>
        /// The service used to consume APIs
        /// </summary>
        private IApiService ApiService { get; }

        #endregion

        #region Property -> Movie

        private MovieFull _movie;
        public MovieFull Movie
        {
            get
            {
                return _movie;
            }
            set
            {
                Set(() => Movie, ref _movie, value);
            }
        }
        #endregion

        #region Property -> CancellationDownloadingSubtitlesToken

        /// <summary>
        /// Token to cancel downloading subtitles
        /// </summary>
        private CancellationTokenSource CancellationDownloadingSubtitlesToken { get; set; }

        #endregion

        #endregion

        public SubtitlesViewModel(MovieFull movie)
        {
            ApiService = SimpleIoc.Default.GetInstance<IApiService>();
            Movie = movie;
            CancellationDownloadingSubtitlesToken = new CancellationTokenSource();

            Task.Run(async () =>
            {
                await LoadSubtitlesAsync(Movie);
            });
        }

        #region Method -> LoadSubtitlesAsync
        /// <summary>
        /// Get the movie's subtitles
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <returns></returns>
        private async Task LoadSubtitlesAsync(MovieFull movie)
        {
            await ApiService.LoadSubtitlesAsync(movie, CancellationDownloadingSubtitlesToken.Token);
        }
        #endregion

        #region Method -> StopDownloadingSubtitles

        /// <summary>
        /// Stop downloading subtitles
        /// </summary>
        public void StopDownloadingSubtitles()
        {
            CancellationDownloadingSubtitlesToken?.Cancel(true);
            CancellationDownloadingSubtitlesToken?.Dispose();
            CancellationDownloadingSubtitlesToken = new CancellationTokenSource();
        }

        #endregion

        public override void Cleanup()
        {
            StopDownloadingSubtitles();
            base.Cleanup();
        }
    }
}
