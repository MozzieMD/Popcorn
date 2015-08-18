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
using Popcorn.Models.Movie;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using Popcorn.Models.Localization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Popcorn.Messaging;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Models.Movie.Json;
using Popcorn.Models.Subtitle;
using Popcorn.Models.Subtitle.Json;

namespace Popcorn.Services.Movie
{
    /// <summary>
    /// Services used to interact with movies
    /// </summary>
    public class MovieService
    {
        #region Logger

        /// <summary>
        /// Logger of the class
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
        public MovieService()
        {
            TmdbClient = new TMDbClient(Constants.TmDbClientId)
            {
                MaxRetryCount = 10,
                RetryWaitTimeInSeconds = 1
            };
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
        /// <returns>Popular movies and the number of movies found</returns>
        public async Task<Tuple<IEnumerable<MovieShort>, int>> GetPopularMoviesAsync(int page,
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
                Messenger.Default.Send(new ManageExceptionMessage(new WebException(response.ErrorException.Message)));

            var wrapper =
                await Task.Run(() => JsonConvert.DeserializeObject<WrapperMovieShortDeserialized>(response.Content), ct);
            if (wrapper == null)
                return new Tuple<IEnumerable<MovieShort>, int>(new List<MovieShort>(), 0);

            var movies = GetMoviesListFromWrapper(wrapper);
            var nbMovies = wrapper.Data.MovieCount;

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"GetPopularMoviesAsync ({page}, {limit}) in {elapsedMs} milliseconds.");

            return new Tuple<IEnumerable<MovieShort>, int>(movies, nbMovies);
        }

        #endregion

        #region Method -> GetTopRatedMoviesAsync

        /// <summary>
        /// Get top rated movies by page
        /// </summary>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Top rated movies and the number of movies found</returns>
        public async Task<Tuple<IEnumerable<MovieShort>, int>> GetTopRatedMoviesAsync(int page,
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
                Messenger.Default.Send(new ManageExceptionMessage(new WebException(response.ErrorException.Message)));

            var wrapper =
                await Task.Run(() => JsonConvert.DeserializeObject<WrapperMovieShortDeserialized>(response.Content), ct);
            if (wrapper == null)
                return new Tuple<IEnumerable<MovieShort>, int>(new List<MovieShort>(), 0);

            var movies = GetMoviesListFromWrapper(wrapper);
            var nbMovies = wrapper.Data.MovieCount;

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"GetTopRatedMoviesAsync ({page}, {limit}) in {elapsedMs} milliseconds.");

            return new Tuple<IEnumerable<MovieShort>, int>(movies, nbMovies);
        }

        #endregion

        #region Method -> GetRecentMoviesAsync

        /// <summary>
        /// Get recent movies by page
        /// </summary>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Recent movies and the number of movies found</returns>
        public async Task<Tuple<IEnumerable<MovieShort>, int>> GetRecentMoviesAsync(int page,
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
                Messenger.Default.Send(new ManageExceptionMessage(new WebException(response.ErrorException.Message)));

            var wrapper =
                await Task.Run(() => JsonConvert.DeserializeObject<WrapperMovieShortDeserialized>(response.Content), ct);
            if (wrapper == null)
                return new Tuple<IEnumerable<MovieShort>, int>(new List<MovieShort>(), 0);

            var movies = GetMoviesListFromWrapper(wrapper);
            var nbMovies = wrapper.Data.MovieCount;

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"GetRecentMoviesAsync ({page}, {limit}) in {elapsedMs} milliseconds.");

            return new Tuple<IEnumerable<MovieShort>, int>(movies, nbMovies);
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
        /// <returns>Searched movies and the number of movies found</returns>
        public async Task<Tuple<IEnumerable<MovieShort>, int>> SearchMoviesAsync(string criteria,
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
                Messenger.Default.Send(new ManageExceptionMessage(new WebException(response.ErrorException.Message)));

            var wrapper =
                await Task.Run(() => JsonConvert.DeserializeObject<WrapperMovieShortDeserialized>(response.Content), ct);
            if (wrapper == null)
                return new Tuple<IEnumerable<MovieShort>, int>(new List<MovieShort>(), 0);

            var movies = GetMoviesListFromWrapper(wrapper);
            var nbMovies = wrapper.Data.MovieCount;

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"SearchMoviesAsync ({criteria}, {page}, {limit}) in {elapsedMs} milliseconds.");

            return new Tuple<IEnumerable<MovieShort>, int>(movies, nbMovies);
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
                Messenger.Default.Send(new ManageExceptionMessage(new WebException(response.ErrorException.Message)));

            var movie = new MovieFull();
            await Task.Run(() =>
            {
                var wrapper = JsonConvert.DeserializeObject<WrapperMovieFullDeserialized>(response.Content);
                if (wrapper == null)
                    return;

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
            Logger.Debug(
                $"GetMovieFullDetails ({ movie.ImdbCode}) in {elapsedMs} milliseconds.");

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

            await Task.Run(() =>
            {
                try
                {
                    var movie = TmdbClient.GetMovie(movieToTranslate.ImdbCode,
                        MovieMethods.Credits);
                    movieToTranslate.Title = movie.Title;
                    movieToTranslate.Genres = movie.Genres.Select(a => a.Name).ToList();
                }
                catch (Exception ex) when (ex is SocketException || ex is WebException)
                {
                    Messenger.Default.Send(new ManageExceptionMessage(ex));
                }
            });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"TranslateMovieShort ({movieToTranslate.ImdbCode}) in {elapsedMs} milliseconds.");
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
            Logger.Debug(
                $"TranslateMovieFull ({movieToTranslate.ImdbCode}) in {elapsedMs} milliseconds.");
        }

        #endregion

        #region Method -> GetMoviesListFromWrapper

        /// <summary>
        /// Get movies as a list from wrapped movies
        /// </summary>
        /// <param name="wrapper">Wrapped movies</param>
        /// <returns>List of movies</returns>
        private static List<MovieShort> GetMoviesListFromWrapper(WrapperMovieShortDeserialized wrapper)
        {
            var movies = new List<MovieShort>();
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
            catch (Exception ex) when (ex is SocketException || ex is WebException)
            {
                Messenger.Default.Send(new ManageExceptionMessage(ex));
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"GetMovieTrailerAsync ({movie.ImdbCode}) in {elapsedMs} milliseconds.");

            return trailers;
        }

        #endregion

        #region Method -> LoadSubtitlesAsync

        /// <summary>
        /// Get the movie's subtitles according to a language
        /// </summary>
        /// <param name="movie">The movie of which to retrieve its subtitles</param>
        /// <param name="ct">Cancellation token</param>
        public async Task LoadSubtitlesAsync(MovieFull movie,
            CancellationToken ct)
        {
            var restClient = new RestClient(Constants.YifySubtitlesApi);
            var request = new RestRequest("/{segment}", Method.GET);
            request.AddUrlSegment("segment", movie.ImdbCode);

            var response = await restClient.ExecuteGetTaskAsync(request, ct);
            if (response.ErrorException != null)
                Messenger.Default.Send(new ManageExceptionMessage(new Exception(response.ErrorException.Message)));

            var wrapper =
                await Task.Run(() => JsonConvert.DeserializeObject<SubtitlesWrapperDeserialized>(response.Content), ct);

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
            movie.AvailableSubtitles = subtitles;
        }

        #endregion

        #region Method -> DownloadSubtitleAsync

        /// <summary>
        /// Download a subtitle
        /// </summary>
        /// <param name="movie">The movie of which to retrieve its subtitles</param>
        /// <param name="progress">Report the progress of the download</param>
        /// <param name="ct">Cancellation token</param>
        public async Task DownloadSubtitleAsync(MovieFull movie, IProgress<long> progress, CancellationTokenSource ct)
        {
            if (movie.SelectedSubtitle == null)
                return;

            var filePath = Constants.Subtitles + movie.ImdbCode + "\\" + movie.SelectedSubtitle.Language.EnglishName +
                           ".zip";

            try
            {
                var result = await
                    DownloadFileHelper.DownloadFileTaskAsync(
                        Constants.YifySubtitles + movie.SelectedSubtitle.Url, filePath, 10000, progress, ct);

                if (result.Item3 == null && !string.IsNullOrEmpty(result.Item2))
                {
                    using (var archive = ZipFile.OpenRead(result.Item2))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (!entry.FullName.StartsWith("_") &&
                                entry.FullName.EndsWith(".srt", StringComparison.OrdinalIgnoreCase))
                            {
                                var subtitlePath = Path.Combine(Constants.Subtitles + movie.ImdbCode,
                                    entry.FullName);
                                if (!File.Exists(subtitlePath))
                                {
                                    entry.ExtractToFile(subtitlePath);
                                }

                                movie.SelectedSubtitle.FilePath = subtitlePath;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"DownloadSubtitleAsync (failed): {movie.Title}. Additional informations : {ex.Message}");
            }
        }

        #endregion

        #region Method -> DownloadBackgroundImageAsync

        /// <summary>
        /// Download the movie's background image
        /// </summary>
        /// <param name="movie">The movie to process</param>
        public async Task DownloadBackgroundImageAsync(MovieFull movie)
        {
            var watch = Stopwatch.StartNew();
            await Task.Run(async () =>
            {
                try
                {
                    TmdbClient.GetConfig();
                    var tmdbMovie = TmdbClient.GetMovie(movie.ImdbCode, MovieMethods.Images);
                    var remotePath = new List<string>
                    {
                        TmdbClient.GetImageUrl(Constants.BackgroundImageSizeTmDb,
                            tmdbMovie.BackdropPath).AbsoluteUri
                    };

                    await
                        remotePath.ForEachAsync(
                            background =>
                                DownloadFileHelper.DownloadFileTaskAsync(background,
                                    Constants.BackgroundMovieDirectory + movie.ImdbCode + Constants.ImageFileExtension),
                            (background, t) =>
                            {
                                if (t.Item3 == null && !string.IsNullOrEmpty(t.Item2))
                                {
                                    movie.BackgroundImagePath = t.Item2;
                                }
                            });
                }
                catch (Exception ex) when (ex is SocketException || ex is WebException)
                {
                    Messenger.Default.Send(new ManageExceptionMessage(ex));
                }
            });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"DownloadBackgroundImageAsync ({movie.ImdbCode}) in {elapsedMs} milliseconds.");
        }

        #endregion

        #region Method -> DownloadCoverImageAsync

        /// <summary>
        /// Download cover image for each of the movies provided
        /// </summary>
        /// <param name="movies">The movies to process</param>
        public async Task DownloadCoverImageAsync(IEnumerable<MovieShort> movies)
        {
            var watch = Stopwatch.StartNew();

            var moviesToProcess = movies.ToList();

            await
                moviesToProcess.ForEachAsync(
                    movie =>
                        DownloadFileHelper.DownloadFileTaskAsync(movie.MediumCoverImage,
                            Constants.CoverMoviesDirectory + movie.ImdbCode + Constants.ImageFileExtension),
                    (movie, t) =>
                    {
                        if (t.Item3 == null && !string.IsNullOrEmpty(t.Item2))
                        {
                            movie.CoverImagePath = t.Item2;
                        }
                    });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"DownloadCoverImageAsync ({string.Join(";", moviesToProcess.Select(movie => movie.ImdbCode))}) in {elapsedMs} milliseconds.");
        }

        #endregion

        #region Method -> DownloadPosterImageAsync

        /// <summary>
        /// Download the movie's poster image
        /// </summary>
        /// <param name="movie">The movie to process</param>
        public async Task DownloadPosterImageAsync(MovieFull movie)
        {
            var watch = Stopwatch.StartNew();

            if (movie.Images == null)
                return;

            var posterPath = new List<string>
            {
                movie.Images.LargeCoverImage
            };

            await
                posterPath.ForEachAsync(
                    poster =>
                        DownloadFileHelper.DownloadFileTaskAsync(poster,
                            Constants.PosterMovieDirectory + movie.ImdbCode + Constants.ImageFileExtension),
                    (poster, t) =>
                    {
                        if (t.Item3 == null && !string.IsNullOrEmpty(t.Item2))
                        {
                            movie.PosterImagePath = t.Item2;
                        }
                    });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"DownloadPosterImageAsync ({movie.ImdbCode}) in {elapsedMs} milliseconds.");
        }

        #endregion

        #region Method -> DownloadDirectorImageAsync

        /// <summary>
        /// Download directors' image for a movie
        /// </summary>
        /// <param name="movie">The movie to process</param>
        public async Task DownloadDirectorImageAsync(MovieFull movie)
        {
            var watch = Stopwatch.StartNew();

            if (movie.Directors == null)
                return;

            await
                movie.Directors.ForEachAsync(
                    director =>
                        DownloadFileHelper.DownloadFileTaskAsync(director.SmallImage,
                            Constants.DirectorMovieDirectory + director.Name + Constants.ImageFileExtension),
                    (director, t) =>
                    {
                        if (t.Item3 == null && !string.IsNullOrEmpty(t.Item2))
                        {
                            director.SmallImagePath = t.Item2;
                        }
                    });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"DownloadDirectorImageAsync ({movie.ImdbCode}) in {elapsedMs} milliseconds.");
        }

        #endregion

        #region Method -> DownloadActorImageAsync

        /// <summary>
        /// Download actors' image for a movie
        /// </summary>
        /// <param name="movie">The movie to process</param>
        public async Task DownloadActorImageAsync(MovieFull movie)
        {
            var watch = Stopwatch.StartNew();

            if (movie.Actors == null)
                return;

            await
                movie.Actors.ForEachAsync(
                    actor =>
                        DownloadFileHelper.DownloadFileTaskAsync(actor.SmallImage,
                            Constants.ActorMovieDirectory + actor.Name + Constants.ImageFileExtension),
                    (actor, t) =>
                    {
                        if (t.Item3 == null && !string.IsNullOrEmpty(t.Item2))
                        {
                            actor.SmallImagePath = t.Item2;
                        }
                    });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"DownloadActorImageAsync ({movie.ImdbCode}) in {elapsedMs} milliseconds.");
        }

        #endregion

        #endregion
    }
}