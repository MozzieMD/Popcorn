using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using Popcorn.Model.Subtitle;
using Popcorn.Service.Api;
using Ragnar;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Popcorn.ViewModel.Download
{
    public class DownloadMovieViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> ApiService

        /// <summary>
        /// The service used to consume APIs
        /// </summary>
        private IApiService ApiService { get; }

        #endregion

        #region Property -> Subtitles

        /// <summary>
        /// The available movie's subtitles
        /// </summary>
        private ObservableCollection<Subtitle> _subtitles = new ObservableCollection<Subtitle>();

        public ObservableCollection<Subtitle> Subtitles
        {
            get { return _subtitles; }
            set { Set(() => Subtitles, ref _subtitles, value); }
        }

        #endregion

        #region Property -> Subtitle

        /// <summary>
        /// The current movie's subtitle
        /// </summary>
        private Subtitle _subtitle;

        public Subtitle Subtitle
        {
            get { return _subtitle; }
            set { Set(() => Subtitle, ref _subtitle, value); }
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

        #region Property -> IsMovieBuffered

        /// <summary>
        /// Specify if a movie is buffered
        /// </summary>
        private bool _isMovieBuffered;

        /// <summary>
        /// Specify if a movie is downloading
        /// </summary>
        public bool IsMovieBuffered
        {
            get { return _isMovieBuffered; }
            set { Set(() => IsMovieBuffered, ref _isMovieBuffered, value); }
        }

        #endregion

        #region Property -> DownloadProgress

        /// <summary>
        /// Specify the movie download progress
        /// </summary>
        private double _downloadProgress;

        /// <summary>
        /// Specify the movie download progress
        /// </summary>
        public double DownloadProgress
        {
            get { return _downloadProgress; }
            set { Set(() => DownloadProgress, ref _downloadProgress, value); }
        }

        #endregion

        #region Property -> DownloadText

        /// <summary>
        /// Text indication on downloading a movie
        /// </summary>
        private string _downloadText;

        /// <summary>
        /// Text indication on downloading a movie
        /// </summary>
        public string DownloadText
        {
            get { return _downloadText; }
            set { Set(() => DownloadText, ref _downloadText, value); }
        }

        #endregion

        #region Property -> CancellationDownloadingMovieToken

        /// <summary>
        /// Token to cancel movie downloading
        /// </summary>
        private CancellationTokenSource CancellationDownloadingMovieToken { get; set; }

        #endregion

        #endregion

        #region Commands

        #region Command -> StopPlayingMovieCommand

        /// <summary>
        /// StopPlayingMovieCommand
        /// </summary>
        public RelayCommand StopPlayingMovieCommand { get; private set; }

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="movie">The movie to download</param>
        public DownloadMovieViewModel(MovieFull movie)
        {
            CancellationDownloadingMovieToken = new CancellationTokenSource();

            ApiService = SimpleIoc.Default.GetInstance<IApiService>();

            // Stop playing a movie
            StopPlayingMovieCommand = new RelayCommand(() =>
            {
                StopPlayingMovie();
                Messenger.Default.Send(new StopPlayingMovieMessage());
            });

            Task.Run(async () =>
            {
                Subtitles = await GetSubtitlesAsync(movie);
                await DownloadMovieAsync(movie);
            });
        }
        #endregion

        #region Method -> GetSubtitlesAsync
        /// <summary>
        /// Get the movie's subtitles
        /// </summary>
        /// <param name="movie">The movie </param>
        /// <returns></returns>
        private async Task<ObservableCollection<Subtitle>> GetSubtitlesAsync(MovieFull movie)
        {
            return await ApiService.GetSubtitlesAsync(movie, CancellationDownloadingMovieToken.Token);
        }
        #endregion

        #region Method -> DownloadMovieAsync

        /// <summary>
        /// Download a movie
        /// </summary>
        /// <param name="movie">The movie to download</param>
        private async Task DownloadMovieAsync(MovieFull movie)
        {
            using (var session = new Session())
            {
                IsDownloadingMovie = true;

                var torrentUrl = string.Empty;

                // Listening to a port which is randomly between 6881 and 6889
                session.ListenOn(6881, 6889);
                if (movie.WatchInFullHdQuality)
                {
                    torrentUrl =
                        movie.Torrents?.FirstOrDefault(torrent => torrent.Quality == "1080p")?.Url;
                }
                else
                {
                    torrentUrl = movie.Torrents?.FirstOrDefault(torrent => torrent.Quality == "720p")?.Url;

                }

                var addParams = new AddTorrentParams
                {
                    // Where do we save the video file
                    SavePath = Constants.MovieDownloads,
                    // At this time, no quality selection is available in the interface, so we take the lowest
                    Url = torrentUrl
                };

                var handle = session.AddTorrent(addParams);

                // We have to download sequentially, so that we're able to play the movie without waiting
                handle.SequentialDownload = true;
                var alreadyBuffered = false;
                while (IsDownloadingMovie)
                {
                    var status = handle.QueryStatus();
                    double progress = status.Progress * 100.0;

                    // We have to flush cache regularly (otherwise memory cache get full very quickly)
                    handle.FlushCache();

                    if (handle.NeedSaveResumeData())
                    {
                        handle.SaveResumeData();
                    }

                    DownloadProgress = progress;
                    var percentage = progress;
                    if (percentage >= 2.0)
                    {
                        DownloadText = LocalizationProviderHelper.GetLocalizedValue<string>("CurrentlyPlayingLabel") +
                            " : " +
                            movie.Title;
                        if (!IsMovieBuffered)
                        {
                            IsMovieBuffered = true;
                        }
                    }
                    else
                    {
                        var downloadRateInKb = status.DownloadRate / 1024;
                        if (downloadRateInKb >= 1000)
                        {
                            DownloadText = LocalizationProviderHelper.GetLocalizedValue<string>("BufferingLabel") +
                                " : " +
                                Math.Round(percentage * 50, 0) +
                                " %" +
                                " ( " +
                                downloadRateInKb / 1000 +
                                " MB/s)";
                        }
                        else
                        {
                            DownloadText = LocalizationProviderHelper.GetLocalizedValue<string>("BufferingLabel") +
                                " : " +
                                Math.Round(percentage * 50, 0) +
                                " %" +
                                " ( " +
                                downloadRateInKb +
                                " kB/s)";
                        }
                    }

                    if (progress >= Constants.MinimumBufferingBeforeMoviePlaying && !alreadyBuffered)
                    {
                        // Get movie file
                        foreach (
                            string filePath in
                                Directory.GetFiles(status.SavePath + handle.TorrentFile.Name,
                                    "*" + Constants.VideoFileExtension)
                            )
                        {
                            alreadyBuffered = true;
                            // Inform subscribers we have finished buffering the movie
                            Messenger.Default.Send(new MovieBufferedMessage(movie,
                                new Uri(filePath)));
                        }
                    }

                    // Wait for a second before update torrent status
                    await Task.Delay(1000, CancellationDownloadingMovieToken.Token);
                }
            }
        }

        #endregion

        #region Method -> StopPlayingMovie

        /// <summary>
        /// Stop downloading a movie
        /// </summary>
        public void StopPlayingMovie()
        {
            IsDownloadingMovie = false;
            IsMovieBuffered = false;

            CancellationDownloadingMovieToken?.Cancel(true);
            CancellationDownloadingMovieToken?.Dispose();
            CancellationDownloadingMovieToken = new CancellationTokenSource();
        }

        #endregion

        public override void Cleanup()
        {
            StopPlayingMovie();
            base.Cleanup();
        }
    }
}
