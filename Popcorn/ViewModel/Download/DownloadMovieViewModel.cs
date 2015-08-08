using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using Popcorn.ViewModel.MovieSettings;
using Ragnar;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Popcorn.Service.Movie;

namespace Popcorn.ViewModel.Download
{
    /// <summary>
    /// Manage the download of a movie
    /// </summary>
    public sealed class DownloadMovieViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> MovieService

        /// <summary>
        /// The service used to interact with movies
        /// </summary>
        private IMovieService MovieService { get; }

        #endregion

        #region Property -> MovieSettingsViewModel

        private MovieSettingsViewModel _movieSettings;

        /// <summary>
        /// The view model used to manage movie's settings
        /// </summary>
        public MovieSettingsViewModel MovieSettings
        {
            get { return _movieSettings; }
            set { Set(() => MovieSettings, ref _movieSettings, value); }
        }

        #endregion

        #region Property -> IsDownloadingMovie

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

        #region Property -> DownloadRate

        private double _downloadRate;

        /// <summary>
        /// Specify the movie download rate
        /// </summary>
        public double DownloadRate
        {
            get { return _downloadProgress; }
            set { Set(() => DownloadRate, ref _downloadRate, value); }
        }

        #endregion

        #region Property -> Movie

        private MovieFull _movie;

        /// <summary>
        /// The movie to download
        /// </summary>
        public MovieFull Movie
        {
            get { return _movie; }
            set { Set(() => Movie, ref _movie, value); }
        }

        #endregion

        #region Property -> CancellationDownloadingMovieToken

        /// <summary>
        /// Token to cancel movie downloading
        /// </summary>
        private CancellationTokenSource CancellationDownloadingMovieToken { get; }

        #endregion

        #endregion

        #region Commands

        #region Command -> StopDownloadingMovieCommand

        /// <summary>
        /// The command used to stop the playing of a movie
        /// </summary>
        public RelayCommand StopDownloadingMovieCommand { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DownloadMovieViewModel class.
        /// </summary>
        /// <param name="movie">The movie to download</param>
        public DownloadMovieViewModel(MovieFull movie)
        {
            RegisterMessages();

            RegisterCommands();

            CancellationDownloadingMovieToken = new CancellationTokenSource();

            MovieService = SimpleIoc.Default.GetInstance<IMovieService>();

            Movie = movie;
            MovieSettings = new MovieSettingsViewModel(movie);
        }

        #endregion

        #region Methods

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<DownloadMovieMessage>(
                this,
                async message =>
                {
                    var reportDownloadProgress = new Progress<double>(ReportDownloadProgress);
                    var reportDownloadRate = new Progress<double>(ReportDownloadRate);

                    await
                        MovieService.DownloadSubtitleAsync(message.Movie);
                    await
                        DownloadMovieAsync(message.Movie,
                            reportDownloadProgress, reportDownloadRate, CancellationDownloadingMovieToken.Token);
                });
        }

        #endregion

        #region Method -> RegisterCommands
        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            StopDownloadingMovieCommand = new RelayCommand(StopDownloadingMovie);
        }
        #endregion

        #region Method -> ReportDownloadRate

        /// <summary>
        /// Report the download progress
        /// </summary>
        /// <param name="value"></param>
        private void ReportDownloadRate(double value)
        {
            DownloadRate = value;
        }

        #endregion

        #region Method -> ReportDownloadProgress

        /// <summary>
        /// Report the download progress
        /// </summary>
        /// <param name="value">The value to report</param>
        private void ReportDownloadProgress(double value)
        {
            DownloadProgress = value;
            if (value < Constants.MinimumBufferingBeforeMoviePlaying)
                return;
            
            if (!IsMovieBuffered)
            {
                IsMovieBuffered = true;
            }
        }

        #endregion

        #region Method -> DownloadMovieAsync

        /// <summary>
        /// Download a movie
        /// </summary>
        /// <param name="movie">The movie to download</param>
        /// <param name="downloadProgress">Report download progress</param>
        /// <param name="downloadRate">Report download rate</param>
        /// <param name="ct">Cancellation token</param>
        private async Task DownloadMovieAsync(MovieFull movie, IProgress<double> downloadProgress,
            IProgress<double> downloadRate,
            CancellationToken ct)
        {
            using (var session = new Session())
            {
                IsDownloadingMovie = true;

                // Listening to a port which is randomly between 6881 and 6889
                session.ListenOn(6881, 6889);
                var torrentUrl = movie.WatchInFullHdQuality
                    ? movie.Torrents?.FirstOrDefault(torrent => torrent.Quality == "1080p")?.Url
                    : movie.Torrents?.FirstOrDefault(torrent => torrent.Quality == "720p")?.Url;

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
                    var progress = status.Progress*100.0;

                    downloadProgress?.Report(progress);
                    downloadRate?.Report(Math.Round((double) status.DownloadRate/1024, 0));

                    // We have to flush cache regularly (otherwise memory cache get full very quickly)
                    handle.FlushCache();
                    if (handle.NeedSaveResumeData())
                    {
                        handle.SaveResumeData();
                    }

                    if (progress >= Constants.MinimumBufferingBeforeMoviePlaying && !alreadyBuffered)
                    {
                        // Get movie file
                        foreach (
                            var filePath in
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

                    try
                    {
                        // Wait for a second before update torrent status
                        await Task.Delay(1000, ct);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region Method -> StopDownloadingMovie

        /// <summary>
        /// Stop downloading a movie
        /// </summary>
        private void StopDownloadingMovie()
        {
            IsDownloadingMovie = false;
            IsMovieBuffered = false;
            CancellationDownloadingMovieToken?.Cancel();
        }

        #endregion

        public override void Cleanup()
        {
            StopDownloadingMovie();
            CancellationDownloadingMovieToken?.Dispose();
            MovieSettings?.Cleanup();
            base.Cleanup();
        }

        #endregion
    }
}