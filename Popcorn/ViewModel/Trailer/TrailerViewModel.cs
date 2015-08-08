using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Popcorn.Service.Movie;
using Popcorn.ViewModel.Players.Trailer;
using YoutubeExtractor;

namespace Popcorn.ViewModel.Trailer
{
    /// <summary>
    /// Manage trailer
    /// </summary>
    public sealed class TrailerViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> MovieService

        /// <summary>
        /// The service used to interact with movies
        /// </summary>
        private IMovieService MovieService { get; }

        #endregion

        #region Property -> Movie

        /// <summary>
        /// Represent the subtitle's movie
        /// </summary>
        private readonly MovieFull Movie;

        #endregion

        #region Property -> StreamingQualityMap

        /// <summary>
        /// Map for defining youtube video quality
        /// </summary>
        private static readonly IReadOnlyDictionary<Constants.YoutubeStreamingQuality, IEnumerable<int>>
            StreamingQualityMap =
                new Dictionary<Constants.YoutubeStreamingQuality, IEnumerable<int>>
                {
                    {Constants.YoutubeStreamingQuality.High, new HashSet<int> {1080, 720}},
                    {Constants.YoutubeStreamingQuality.Medium, new HashSet<int> {480}},
                    {Constants.YoutubeStreamingQuality.Low, new HashSet<int> {360, 240}}
                };

        #endregion

        #region Property -> IsTrailerLoading

        /// <summary>
        /// Specify if a trailer is loading
        /// </summary>
        private bool _isTrailerLoading;

        /// <summary>
        /// Specify if a trailer is loading
        /// </summary>
        public bool IsTrailerLoading
        {
            get { return _isTrailerLoading; }
            set { Set(() => IsTrailerLoading, ref _isTrailerLoading, value); }
        }

        #endregion

        #region Property -> TrailerPlayer

        /// <summary>
        /// The trailer
        /// </summary>
        private TrailerPlayerViewModel _trailerPlayer;

        public TrailerPlayerViewModel TrailerPlayer
        {
            get { return _trailerPlayer; }
            set { Set(() => TrailerPlayer, ref _trailerPlayer, value); }
        }

        #endregion

        #region Property -> CancellationLoadingTrailerToken

        /// <summary>
        /// Token to cancel trailer loading
        /// </summary>
        private CancellationTokenSource CancellationLoadingTrailerToken { get; }

        #endregion

        #region Commands

        #region Command -> StopLoadingTrailerCommand

        /// <summary>
        /// Stop loading the trailer
        /// </summary>
        public RelayCommand StopLoadingTrailerCommand { get; private set; }

        #endregion

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TrailerViewModel class.
        /// </summary>
        /// <param name="movie">Movie's trailer</param>
        private TrailerViewModel(MovieFull movie)
        {
            RegisterCommands();

            CancellationLoadingTrailerToken = new CancellationTokenSource();

            MovieService = SimpleIoc.Default.GetInstance<IMovieService>();

            Movie = movie;
        }

        #endregion

        #region Method -> InitializeAsync
        /// <summary>
        /// Load asynchronously the movie's trailer and return an instance of TrailerViewModel
        /// </summary>
        /// <returns>Instance of TrailerViewModel</returns>
        private async Task<TrailerViewModel> InitializeAsync()
        {
            await GetTrailerAsync(Movie);
            return this;
        }
        #endregion

        #region Method -> CreateAsync
        /// <summary>
        /// Initialize asynchronously an instance of the TrailerViewModel class
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <returns>Instance of TrailerViewModel</returns>
        public static Task<TrailerViewModel> CreateAsync(MovieFull movie)
        {
            var ret = new TrailerViewModel(movie);
            return ret.InitializeAsync();
        }
        #endregion

        #region -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            StopLoadingTrailerCommand =
                new RelayCommand(() => { Messenger.Default.Send(new StopPlayingTrailerMessage()); });
        }

        #endregion

        #region Method -> GetTrailerAsync

        /// <summary>
        /// Get trailer of a movie
        /// </summary>
        /// <param name="movie">Movie</param>
        private async Task GetTrailerAsync(MovieFull movie)
        {
            await Task.Run(async () =>
            {
                // Inform subscribers we are loading movie trailer
                IsTrailerLoading = true;
                try
                {
                    // Retrieve trailer from API
                    var trailer = await MovieService.GetMovieTrailerAsync(movie);
                    // No error has been encounter, we can create our VideoInfo
                    VideoInfo video = null;
                    try
                    {
                        // Retrieve Youtube Infos
                        video =
                            await
                                GetVideoInfoForStreamingAsync(
                                    Constants.YoutubePath + trailer.Results.FirstOrDefault()?.Key,
                                    Constants.YoutubeStreamingQuality.High);

                        if (video != null && video.RequiresDecryption)
                        {
                            // Decrypt encoded Youtube video link 
                            await Task.Run(() => DownloadUrlResolver.DecryptDownloadUrl(video));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is WebException || ex is VideoNotAvailableException || ex is YoutubeParseException)
                        {
                            // TODO: Inform loading trailer failed
                            Messenger.Default.Send(new StopPlayingTrailerMessage());
                            return;
                        }
                    }

                    if (video == null)
                    {
                        // TODO: Inform loading trailer failed
                        Messenger.Default.Send(new StopPlayingTrailerMessage());
                        return;
                    }

                    if (!CancellationLoadingTrailerToken.IsCancellationRequested)
                    {
                        // Inform we have loaded trailer successfully
                        TrailerPlayer = new TrailerPlayerViewModel(new Uri(video.DownloadUrl));
                        IsTrailerLoading = false;
                    }
                }
                catch (MovieServiceException e)
                {
                    Messenger.Default.Send(new StopPlayingTrailerMessage());
                    if (e.Status == MovieServiceException.State.ConnectionError)
                    {
                        Messenger.Default.Send(new ConnectionErrorMessage(e.Message));
                    }
                }

            }, CancellationLoadingTrailerToken.Token);
        }

        #endregion

        #region Method -> GetVideoInfoForStreamingAsync

        /// <summary>
        /// Get VideoInfo of a youtube video
        /// </summary>
        /// <param name="youtubeLink">The youtube link of a movie</param>
        /// <param name="qualitySetting">The youtube quality settings</param>
        private async Task<VideoInfo> GetVideoInfoForStreamingAsync(string youtubeLink,
            Constants.YoutubeStreamingQuality qualitySetting)
        {
            // Get video infos of the requested video
            var videoInfos = await Task.Run(() => DownloadUrlResolver.GetDownloadUrls(youtubeLink, false));

            // We only want video matching criterias : only mp4 and no adaptive
            var filtered = videoInfos
                .Where(info => info.VideoType == VideoType.Mp4 && !info.Is3D && info.AdaptiveType == AdaptiveType.None);

            return GetVideoByStreamingQuality(filtered, qualitySetting);
        }

        #endregion

        #region Method -> GetVideoByStreamingQuality

        /// <summary>
        /// Get youtube video depending of choosen quality settings
        /// </summary>
        /// <param name="videos">List of VideoInfo</param>
        /// <param name="quality">The youtube quality settings</param>
        private VideoInfo GetVideoByStreamingQuality(IEnumerable<VideoInfo> videos,
            Constants.YoutubeStreamingQuality quality)
        {
            videos = videos.ToList(); // Prevent multiple enumeration

            if (quality == Constants.YoutubeStreamingQuality.High)
            {
                // Choose high quality Youtube video
                return videos.OrderByDescending(x => x.Resolution)
                    .FirstOrDefault();
            }

            // Pick the video with the requested quality settings
            var preferredResolutions = StreamingQualityMap[quality];

            var preferredVideos = videos
                .Where(info => preferredResolutions.Contains(info.Resolution))
                .OrderByDescending(info => info.Resolution);

            var video = preferredVideos.FirstOrDefault();

            if (video == null)
            {
                // We search for an other video quality if none has been found
                return GetVideoByStreamingQuality(videos, (Constants.YoutubeStreamingQuality) (((int) quality) - 1));
            }

            return video;
        }

        #endregion


        #region Method -> StopLoadingTrailer

        /// <summary>
        /// Stop loading a trailer
        /// </summary>
        private void StopLoadingTrailer()
        {
            IsTrailerLoading = false;
            CancellationLoadingTrailerToken?.Cancel();
        }

        #endregion

        public override void Cleanup()
        {
            StopLoadingTrailer();
            CancellationLoadingTrailerToken?.Dispose();

            TrailerPlayer?.Cleanup();

            base.Cleanup();
        }
    }
}