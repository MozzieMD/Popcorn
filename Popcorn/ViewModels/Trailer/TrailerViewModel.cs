using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Models.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Popcorn.Services.Movie;
using Popcorn.ViewModels.Players.Trailer;
using YoutubeExtractor;

namespace Popcorn.ViewModels.Trailer
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
        private MovieService MovieService { get; }

        #endregion

        #region Property -> Movie

        /// <summary>
        /// Represent the subtitle's movie
        /// </summary>
        private MovieFull Movie { get; }

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

        #region Property -> TrailerPlayer

        private TrailerPlayerViewModel _trailerPlayer;

        /// <summary>
        /// The trailer player
        /// </summary>
        public TrailerPlayerViewModel TrailerPlayer
        {
            get { return _trailerPlayer; }
            set { Set(() => TrailerPlayer, ref _trailerPlayer, value); }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TrailerViewModel class.
        /// </summary>
        /// <param name="movie">Movie's trailer</param>
        private TrailerViewModel(MovieFull movie)
        {
            MovieService = SimpleIoc.Default.GetInstance<MovieService>();
            Movie = movie;
        }

        #endregion

        #region Method -> InitializeAsync

        /// <summary>
        /// Load asynchronously the movie's trailer for the current instance of TrailerViewModel
        /// </summary>
        /// <returns>Instance of TrailerViewModel</returns>
        private async Task<TrailerViewModel> InitializeAsync(CancellationTokenSource ct)
        {
            await LoadTrailerAsync(Movie, ct);
            return this;
        }

        #endregion

        #region Method -> CreateAsync

        /// <summary>
        /// Initialize asynchronously an instance of the TrailerViewModel class
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Instance of TrailerViewModel</returns>
        public static Task<TrailerViewModel> CreateAsync(MovieFull movie, CancellationTokenSource ct)
        {
            var ret = new TrailerViewModel(movie);
            return ret.InitializeAsync(ct);
        }

        #endregion

        #region Method -> LoadTrailerAsync

        /// <summary>
        /// Get trailer of a movie
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <param name="ct">Cancellation token</param>
        private async Task LoadTrailerAsync(MovieFull movie, CancellationTokenSource ct)
        {
            await Task.Run(async () =>
            {
                // Inform subscribers we are loading movie trailer
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
                            if(ex is VideoNotAvailableException)
                                Messenger.Default.Send(new ManageExceptionMessage(new Exception(LocalizationProviderHelper.GetLocalizedValue<string>("TrailerNotAvailable"))));
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

                    if (!ct.IsCancellationRequested)
                    {
                        // Inform we have loaded trailer successfully
                        TrailerPlayer = new TrailerPlayerViewModel(new Models.Trailer.Trailer(new Uri(video.DownloadUrl)));
                    }
                }
                catch (WebException e)
                {
                    Messenger.Default.Send(new StopPlayingTrailerMessage());
                    Messenger.Default.Send(new ManageExceptionMessage(e));
                }
                catch (Exception)
                {
                    Messenger.Default.Send(new StopPlayingTrailerMessage());
                }
            }, ct.Token);
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
        /// <param name="videosToProcess">List of VideoInfo</param>
        /// <param name="quality">The youtube quality settings</param>
        private VideoInfo GetVideoByStreamingQuality(IEnumerable<VideoInfo> videosToProcess,
            Constants.YoutubeStreamingQuality quality)
        {
            var videos = videosToProcess.ToList(); // Prevent multiple enumeration

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

        public override void Cleanup()
        {
            TrailerPlayer?.Cleanup();

            base.Cleanup();
        }
    }
}