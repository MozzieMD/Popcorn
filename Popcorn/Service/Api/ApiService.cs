using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using Popcorn.Helpers;
using Popcorn.Model.Movie;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using Popcorn.Model.Localization;
using System.Linq;
using Popcorn.Messaging;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Model.Movie.Json;
using Popcorn.Model.Subtitle;
using Popcorn.Model.Subtitle.Json;

namespace Popcorn.Service.Api
{
    /// <summary>
    /// Service used to interact with APIs
    /// </summary>
    public class ApiService : IApiService
    {
        #region Logger
        /// <summary>
        /// Logger of the class
        /// </summary>
        private readonly static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region TMDbClient
        /// <summary>
        ///TMDb client
        /// </summary>
        private TMDbClient TmdbClient { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ApiService()
        {
            TmdbClient = new TMDbClient(Constants.TmDbClientId);
            TmdbClient.GetConfig();
            TmdbClient.MaxRetryCount = 10;
            TmdbClient.RetryWaitTimeInSeconds = 1;
        }
        #endregion

        #region Methods

        #region Method -> ChangeTmdbLanguage
        /// <summary>
        /// Change the culture of TMDb
        /// </summary>
        /// <param name="language">Language to set</param>
        public void ChangeTmdbLanguage(ILanguage language)
        {
            TmdbClient.DefaultLanguage = language.Culture;
        }
        #endregion

        #region Method -> GetPopularMoviesAsync
        /// <summary>
        /// Get popular movies by page
        /// </summary>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Popular movies</returns>
        public async Task<IEnumerable<MovieShort>> GetPopularMoviesAsync(int page,
            int limit,
            CancellationToken ct)
        {
            var watch = Stopwatch.StartNew();

            if (limit < 1 || limit > 50)
            {
                limit = 20;
            }

            if (page < 1)
            {
                page = 1;
            }

            var restClient = new RestClient(Constants.YtsApiEndpoint);
            var request = new RestRequest("/{segment}", Method.GET);
            request.AddUrlSegment("segment", "list_movies.json");
            request.AddParameter("limit", limit);
            request.AddParameter("page", page);
            request.AddParameter("sort_by", "like_count");

            var response = await restClient.ExecuteGetTaskAsync(request, ct);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var apiServiceException = new ApiServiceException(message, response.ErrorException);
                throw apiServiceException;
            }

            var wrapper = await Task.Run(() => JsonConvert.DeserializeObject<WrapperMovieShortDeserialized>(response.Content), ct);
            var movies = GetMoviesListFromWrapper(wrapper);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "GetPopularMoviesAsync ({0}, {1}) in {2} milliseconds.", page, limit, elapsedMs);

            return movies;
        }
        #endregion

        #region Method -> GetTopRatedMoviesAsync
        /// <summary>
        /// Get top rated movies by page
        /// </summary>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Top rated movies</returns>
        public async Task<IEnumerable<MovieShort>> GetTopRatedMoviesAsync(int page,
            int limit,
            CancellationToken ct)
        {
            var watch = Stopwatch.StartNew();

            if (limit < 1 || limit > 50)
            {
                limit = 20;
            }

            if (page < 1)
            {
                page = 1;
            }

            var restClient = new RestClient(Constants.YtsApiEndpoint);
            var request = new RestRequest("/{segment}", Method.GET);
            request.AddUrlSegment("segment", "list_movies.json");
            request.AddParameter("limit", limit);
            request.AddParameter("page", page);
            request.AddParameter("sort_by", "rating");

            var response = await restClient.ExecuteGetTaskAsync(request, ct);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var apiServiceException = new ApiServiceException(message, response.ErrorException);
                throw apiServiceException;
            }

            var wrapper = await Task.Run(() => JsonConvert.DeserializeObject<WrapperMovieShortDeserialized>(response.Content), ct);
            var movies = GetMoviesListFromWrapper(wrapper);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "GetTopRatedMoviesAsync ({0}, {1}) in {2} milliseconds.", page, limit, elapsedMs);

            return movies;
        }
        #endregion

        #region Method -> GetRecentMoviesAsync
        /// <summary>
        /// Get recent movies by page
        /// </summary>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Recent movies</returns>
        public async Task<IEnumerable<MovieShort>> GetRecentMoviesAsync(int page,
            int limit,
            CancellationToken ct)
        {
            var watch = Stopwatch.StartNew();

            if (limit < 1 || limit > 50)
            {
                limit = 20;
            }

            if (page < 1)
            {
                page = 1;
            }

            var restClient = new RestClient(Constants.YtsApiEndpoint);
            var request = new RestRequest("/{segment}", Method.GET);
            request.AddUrlSegment("segment", "list_movies.json");
            request.AddParameter("limit", limit);
            request.AddParameter("page", page);
            request.AddParameter("sort_by", "year");

            var response = await restClient.ExecuteGetTaskAsync(request, ct);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var apiServiceException = new ApiServiceException(message, response.ErrorException);
                throw apiServiceException;
            }

            var wrapper = await Task.Run(() => JsonConvert.DeserializeObject<WrapperMovieShortDeserialized>(response.Content), ct);
            var movies = GetMoviesListFromWrapper(wrapper);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "GetRecentMoviesAsync ({0}, {1}) in {2} milliseconds.", page, limit, elapsedMs);

            return movies;
        }
        #endregion

        #region Method -> SearchMoviesAsync
        /// <summary>
        /// Search movies by criteria
        /// </summary>
        /// <param name="criteria">Criteria used for search</param>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Searched movies</returns>
        public async Task<IEnumerable<MovieShort>> SearchMoviesAsync(string criteria,
            int page,
            int limit,
            CancellationToken ct)
        {
            var watch = Stopwatch.StartNew();

            if (limit < 1 || limit > 50)
            {
                limit = 20;
            }

            if (page < 1)
            {
                page = 1;
            }

            var restClient = new RestClient(Constants.YtsApiEndpoint);
            var request = new RestRequest("/{segment}", Method.GET);
            request.AddUrlSegment("segment", "list_movies.json");
            request.AddParameter("limit", limit);
            request.AddParameter("page", page);
            request.AddParameter("query_term", criteria);

            var response = await restClient.ExecuteGetTaskAsync(request, ct);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var apiServiceException = new ApiServiceException(message, response.ErrorException);
                throw apiServiceException;
            }

            var wrapper = await Task.Run(() => JsonConvert.DeserializeObject<WrapperMovieShortDeserialized>(response.Content), ct);
            var movies = GetMoviesListFromWrapper(wrapper);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "SearchMoviesAsync ({0}, {1}, {2}) in {3} milliseconds.", criteria, page, limit, elapsedMs);

            return movies;
        }
        #endregion

        #region Method -> GetMovieFullDetailsAsync
        /// <summary>
        /// Get TMDb movie informations
        /// </summary>
        /// <param name="movieToLoad">Movie to load</param>
        /// <returns>Movie's full details</returns>
        public async Task<MovieFull> GetMovieFullDetailsAsync(MovieShort movieToLoad)
        {
            var watch = Stopwatch.StartNew();

            var restClient = new RestClient(Constants.YtsApiEndpoint);
            var request = new RestRequest("/{segment}", Method.GET);
            request.AddUrlSegment("segment", "movie_details.json");
            request.AddParameter("movie_id", movieToLoad.Id);
            request.AddParameter("with_images", true);
            request.AddParameter("with_cast", true);

            var response = await restClient.ExecuteGetTaskAsync(request);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var apiServiceException = new ApiServiceException(message, response.ErrorException);
                throw apiServiceException;
            }

            var movie = new MovieFull();
            await Task.Run(() => 
            {
                var wrapper = JsonConvert.DeserializeObject<WrapperMovieFullDeserialized>(response.Content);
                var tmdbInfos = TmdbClient.GetMovie(wrapper.Movie.ImdbCode,
                    MovieMethods.Credits);
                movie = new MovieFull
                {
                    Id = wrapper.Movie.Id,
                    Actors = wrapper.Movie.Actors,
                    BackgroundImagePath = string.Empty,
                    DateUploaded = wrapper.Movie.DateUploaded,
                    DateUploadedUnix = wrapper.Movie.DateUploadedUnix,
                    DescriptionFull = tmdbInfos.Overview,
                    DescriptionIntro = wrapper.Movie.DescriptionIntro,
                    Directors = wrapper.Movie.Directors,
                    DownloadCount = wrapper.Movie.DownloadCount,
                    FullHdAvailable = wrapper.Movie.Torrents.Any(torrent => torrent.Quality == "1080p"),
                    Genres = tmdbInfos.Genres.Select(a => a.Name).ToList(),
                    Images = wrapper.Movie.Images,
                    ImdbCode = wrapper.Movie.ImdbCode,
                    Language = wrapper.Movie.Language,
                    LikeCount = wrapper.Movie.LikeCount,
                    MpaRating = wrapper.Movie.MpaRating,
                    PosterImagePath = string.Empty,
                    Rating = wrapper.Movie.Rating,
                    RtAudienceRating = wrapper.Movie.RtAudienceRating,
                    RtAudienceScore = wrapper.Movie.RtAudienceScore,
                    RtCriticsRating = wrapper.Movie.RtCriticsRating,
                    RtCrtiticsScore = wrapper.Movie.RtCrtiticsScore,
                    Runtime = wrapper.Movie.Runtime,
                    Title = tmdbInfos.Title,
                    TitleLong = wrapper.Movie.TitleLong,
                    Torrents = wrapper.Movie.Torrents,
                    Url = wrapper.Movie.Url,
                    WatchInFullHdQuality = false,
                    Year = wrapper.Movie.Year,
                    YtTrailerCode = wrapper.Movie.YtTrailerCode
                };
            });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "GetMovieFullDetails ({0}) in {1} milliseconds.", movie.ImdbCode, elapsedMs);

            return movie;
        }
        #endregion

        #region Method -> TranslateMovieShortAsync
        /// <summary>
        /// Translate movie informations (title, description, ...)
        /// </summary>
        /// <param name="movieToTranslate">Movie to translate</param>
        /// <returns>Task</returns>
        public async Task TranslateMovieShortAsync(MovieShort movieToTranslate)
        {
            var watch = Stopwatch.StartNew();

            Movie movie = null;
            await Task.Run(() =>
            {
                movie = TmdbClient.GetMovie(movieToTranslate.ImdbCode,
                    MovieMethods.Credits);
            });
            movieToTranslate.Title = movie.Title;
            movieToTranslate.Genres = movie.Genres.Select(a => a.Name).ToList();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "TranslateMovieShort ({0}) in {1} milliseconds.", movieToTranslate.ImdbCode, elapsedMs);
        }
        #endregion

        #region Method -> TranslateMovieFullAsync
        /// <summary>
        /// Translate movie informations (title, description, ...)
        /// </summary>
        /// <param name="movieToTranslate">Movie to translate</param>
        /// <returns>Task</returns>
        public async Task TranslateMovieFullAsync(MovieFull movieToTranslate)
        {
            var watch = Stopwatch.StartNew();

            await Task.Run(() =>
            {
                var movie = TmdbClient.GetMovie(movieToTranslate.ImdbCode,
                    MovieMethods.Credits);
                movieToTranslate.Title = movie.Title;
                movieToTranslate.Genres = movie.Genres.Select(a => a.Name).ToList();
                movieToTranslate.DescriptionFull = movie.Overview;
            });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "TranslateMovieFull ({0}) in {1} milliseconds.", movieToTranslate.ImdbCode, elapsedMs);
        }
        #endregion

        #region Method -> GetMoviesListFromWrapper
        /// <summary>
        /// Get movies as a list from wrapped movies
        /// </summary>
        /// <param name="wrapper">Wrapped movies</param>
        /// <returns>List of movies</returns>
        private List<MovieShort> GetMoviesListFromWrapper(WrapperMovieShortDeserialized wrapper)
        {
            List<MovieShort> movies = new List<MovieShort>();
            foreach (var movie in wrapper.Data.Movies)
            {
                movies.Add(new MovieShort
                {
                    ApiVersion = movie.ApiVersion,
                    DateUploaded = movie.DateUploaded,
                    DateUploadedUnix = movie.DateUploadedUnix,
                    ExecutionTime = movie.ExecutionTime,
                    Genres = movie.Genres,
                    Id = movie.Id,
                    ImdbCode = movie.ImdbCode,
                    IsLiked = null,
                    IsSeen = null,
                    Language = movie.Language,
                    MediumCoverImage = movie.MediumCoverImage,
                    CoverImagePath = string.Empty,
                    MpaRating = movie.MpaRating,
                    Rating = movie.Rating,
                    Runtime = movie.Runtime,
                    ServerTime = movie.ServerTime,
                    ServerTimezone = movie.ServerTimezone,
                    SmallCoverImage = movie.SmallCoverImage,
                    State = movie.State,
                    Title = movie.Title,
                    TitleLong = movie.TitleLong,
                    Torrents = movie.Torrents,
                    Url = movie.Url,
                    Year = movie.Year
                });
            }

            return movies;
        }
        #endregion

        #region Method -> GetMovieTrailerAsync
        /// <summary>
        /// Get the link to the youtube trailer of a movie
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <returns>Video trailer</returns>
        public async Task<ResultContainer<Video>> GetMovieTrailerAsync(MovieFull movie)
        {
            var watch = Stopwatch.StartNew();
            var trailers = new ResultContainer<Video>();
            try
            {
                await Task.Run(() =>
                {
                    trailers = TmdbClient.GetMovie(movie.ImdbCode, MovieMethods.Videos)?.Videos;
                });
            }
            catch (ApiServiceException e)
            {
                if (e.Status == ApiServiceException.State.ConnectionError)
                {
                    Messenger.Default.Send(new ConnectionErrorMessage(e.Message));
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "GetMovieTrailerAsync ({0}) in {1} milliseconds.", movie.ImdbCode, elapsedMs);

            return trailers;
        }

        #endregion

        #region Method -> GetSubtitlesAsync
        /// <summary>
        /// Get the movie's subtitles according to a language
        /// </summary>
        /// <param name="movie">The movie of which to retrieve its subtitles</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Movie's subtitles</returns>
        public async Task<ObservableCollection<Subtitle>> GetSubtitlesAsync(MovieFull movie,
            CancellationToken ct)
        {
            var restClient = new RestClient(Constants.YifySubtitlesApi);
            var request = new RestRequest("/{segment}", Method.GET);
            request.AddUrlSegment("segment", movie.ImdbCode);

            var response = await restClient.ExecuteGetTaskAsync(request, ct);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var apiServiceException = new ApiServiceException(message, response.ErrorException);
                throw apiServiceException;
            }
            var wrapper = await Task.Run(() => JsonConvert.DeserializeObject<SubtitlesWrapperDeserialized>(response.Content), ct);
            var subtitles = new ObservableCollection<Subtitle>();
            Dictionary<string, List<SubtitleDeserialized>> movieSubtitles;
            if (wrapper.Subtitles.TryGetValue(movie.ImdbCode, out movieSubtitles))
            {
                foreach (var subtitle in movieSubtitles)
                {
                    var sub = subtitle.Value.Aggregate((sub1, sub2) => sub1.Rating > sub2.Rating ? sub1 : sub2);
                    subtitles.Add(new Subtitle
                    {
                        Id = sub.Id,
                        Language = new CustomLanguage
                        {
                            Culture = string.Empty,
                            EnglishName = subtitle.Key,
                            LocalizedName = string.Empty
                        },
                        Hi = sub.Hi,
                        Rating = sub.Rating,
                        Url = sub.Url
                    });
                }
            }
            subtitles.Sort();

            return subtitles;
        }

        #endregion

        #region Method -> DownloadSubtitleAsync
        /// <summary>
        /// Download a subtitle
        /// </summary>
        /// <param name="movie">The movie of which to retrieve its subtitles</param>
        /// <param name="subtitle">The movie's subtitle to retrieve</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Uri to the downloaded subtitle</returns>
        public async Task<Uri> DownloadSubtitleAsync(MovieFull movie,
            Subtitle subtitle,
            CancellationToken ct)
        {
            var filePath = Constants.Subtitles + "/" + movie.ImdbCode + "/" + subtitle.Language.EnglishName + ".zip";
            await
                DownloadFileTaskAsync(ct, Constants.Subtitles + "/" + movie.ImdbCode, filePath,
                    new Uri(Constants.YifySubtitles + subtitle.Url));
            using (var archive = ZipFile.OpenRead(filePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".srt", StringComparison.OrdinalIgnoreCase))
                    {
                        var subtitlePath = Path.Combine(Constants.Subtitles + "/" + movie.ImdbCode,
                            entry.FullName);
                        entry.ExtractToFile(subtitlePath);
                        return new Uri(subtitlePath);
                    }
                }
            }

            return null;
        }

        #endregion

        #region Method -> DownloadBackgroundImageAsync
        /// <summary>
        /// Download the movie's background image
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded background image</returns>
        public async Task<string> DownloadBackgroundImageAsync(string imdbCode,
            CancellationToken ct)
        {
            var watch = Stopwatch.StartNew();

            var movie = TmdbClient.GetMovie(imdbCode, MovieMethods.Images);
            var address = TmdbClient.GetImageUrl(Constants.BackgroundImageSizeTmDb,
                movie.BackdropPath);

            var folder = Constants.BackgroundMovieDirectory;
            var filePath = folder + imdbCode + Constants.ImageFileExtension;
            await DownloadFileTaskAsync(ct, folder, filePath, address);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "DownloadBackgroundImageAsync ({0}, {1}) in {2} milliseconds.", imdbCode, address.AbsoluteUri, elapsedMs);

            return filePath;
        }

        #endregion

        #region Method -> DownloadCoverImageAsync
        /// <summary>
        /// Download the movie's cover image
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="uri">Resource's uri</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded cover image</returns>
        public async Task<string> DownloadCoverImageAsync(string imdbCode,
            Uri uri,
            CancellationToken ct)
        {
            if (string.IsNullOrEmpty(uri.AbsoluteUri))
            {
                throw new ArgumentException();
            }

            var watch = Stopwatch.StartNew();

            var folder = Constants.CoverMoviesDirectory;
            var filePath = folder + imdbCode + Constants.ImageFileExtension;
            await DownloadFileTaskAsync(ct, folder, filePath, uri);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "DownloadCoverImageAsync ({0}, {1}) in {2} milliseconds.", imdbCode, uri.AbsoluteUri, elapsedMs);

            return filePath;
        }

        #endregion

        #region Method -> DownloadPosterImageAsync
        /// <summary>
        /// Download the movie's poster image
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="uri">Resource's uri</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded poster image</returns>
        public async Task<string> DownloadPosterImageAsync(string imdbCode,
            Uri uri,
            CancellationToken ct)
        {
            if (string.IsNullOrEmpty(uri.AbsoluteUri))
            {
                throw new ArgumentException();
            }

            var watch = Stopwatch.StartNew();

            var folder = Constants.PosterMovieDirectory;
            var filePath = folder + imdbCode + Constants.ImageFileExtension;
            await DownloadFileTaskAsync(ct, folder, filePath, uri);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "DownloadPosterImageAsync ({0}, {1}) in {2} milliseconds.", imdbCode, uri.AbsoluteUri, elapsedMs);

            return filePath;
        }

        #endregion

        #region Method -> DownloadDirectorImageAsync
        /// <summary>
        /// Download the director's image profile
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="uri">Resource's uri</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded director's image</returns>
        public async Task<string> DownloadDirectorImageAsync(string imdbCode,
            Uri uri,
            CancellationToken ct)
        {
            if (string.IsNullOrEmpty(uri.AbsoluteUri))
            {
                throw new ArgumentException();
            }

            var watch = Stopwatch.StartNew();

            var folder = Constants.DirectorMovieDirectory;
            var filePath = folder + imdbCode + Constants.ImageFileExtension;
            await DownloadFileTaskAsync(ct, folder, filePath, uri);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "DownloadDirectorImageAsync ({0}, {1}) in {2} milliseconds.", imdbCode, uri.AbsoluteUri, elapsedMs);

            return filePath;
        }

        #endregion

        #region Method -> DownloadActorImageAsync
        /// <summary>
        /// Download the actor's image profile
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="uri">Resource's uri</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded actor's image</returns>
        public async Task<string> DownloadActorImageAsync(string imdbCode,
            Uri uri,
            CancellationToken ct)
        {
            if (string.IsNullOrEmpty(uri.AbsoluteUri))
            {
                throw new ArgumentException();
            }

            var watch = Stopwatch.StartNew();

            var folder = Constants.ActorMovieDirectory;
            var filePath = folder + imdbCode + Constants.ImageFileExtension;
            await DownloadFileTaskAsync(ct, folder, filePath, uri);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "DownloadActorImageAsync ({0}, {1}) in {2} milliseconds.", imdbCode, uri.AbsoluteUri, elapsedMs);

            return filePath;
        }

        #endregion

        #region Method -> DownloadFileTaskAsync
        /// <summary>
        /// DownloadFileTaskAsync
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <param name="folder">Folder in which file will be saved</param>
        /// <param name="filePath">Path to the file to save</param>
        /// <param name="address">Resource's address</param>
        /// <returns>Task</returns>
        private static async Task DownloadFileTaskAsync(CancellationToken ct, string folder, string filePath, Uri address)
        {
            var watch = Stopwatch.StartNew();

            Directory.CreateDirectory(folder);
            using (var webClient = new NoKeepAliveWebClient())
            {
                try
                {
                    ct.Register(webClient.CancelAsync);
                    if (!File.Exists(filePath))
                    {
                        await webClient.DownloadFileTaskAsync(address,
                            filePath);
                        var fi = new FileInfo(filePath);
                        if (fi.Length == 0)
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        var fi = new FileInfo(filePath);
                        if (fi.Length == 0)
                        {
                            File.Delete(filePath);
                            await
                                webClient.DownloadFileTaskAsync(address, filePath);

                            var newfi = new FileInfo(filePath);
                            if (newfi.Length == 0)
                            {
                                throw new Exception();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "DownloadFileTaskAsync ({0}, {1}, {2}) in {3} milliseconds.", folder, filePath, address.AbsoluteUri, elapsedMs);
        }

        #endregion

        #endregion
    }
}
