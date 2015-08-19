using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Popcorn.Entity;
using Popcorn.Entity.Cast;
using Popcorn.Entity.Movie;
using Popcorn.Helpers;
using Popcorn.Models.Account;
using RestSharp;
using MovieFull = Popcorn.Models.Movie.MovieFull;
using MovieShort = Popcorn.Models.Movie.MovieShort;
using Torrent = Popcorn.Models.Torrent.Torrent;

namespace Popcorn.Services.User
{
    /// <summary>
    /// Services used to interacts with user's data
    /// </summary>
    public class UserService
    {
        #region Logger

        /// <summary>
        /// Logger of the class
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        #region Method -> CreateUser

        /// <summary>
        /// Create a Popcorn user account
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="firstname">The first name</param>
        /// <param name="lastname">The last name</param>
        /// <param name="password">The password</param>
        /// <param name="email">The email</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>User</returns>
        public async Task<Models.Account.User> CreateUser(string username, string firstname, string lastname,
            string password, string email, CancellationToken ct)
        {
            var watch = Stopwatch.StartNew();

            var restClient = new RestClient(Constants.PopcornApiEndpoint);
            var request = new RestRequest("/{segment}", Method.POST);
            request.AddUrlSegment("segment", "api/accounts/create");
            request.AddParameter("username", username);
            request.AddParameter("firstname", firstname);
            request.AddParameter("lastname", lastname);
            request.AddParameter("email", email);
            request.AddParameter("password", password);
            request.AddParameter("confirmpassword", password);

            var response = await restClient.ExecutePostTaskAsync(request, ct);
            if (response.ErrorException != null)
            {
                throw new Exception();
            }

            var user =
                await Task.Run(() => JsonConvert.DeserializeObject<Models.Account.User>(response.Content), ct);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"CreateUser ({username}, {firstname}, {lastname}, {password}, {email}) in {elapsedMs} milliseconds.");

            return user;
        }

        #endregion

        #region Method -> Signin

        /// <summary>
        /// Signin with a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>Bearer</returns>
        public async Task<Bearer> Signin(Models.Account.User user, CancellationToken ct)
        {
            var watch = Stopwatch.StartNew();

            var restClient = new RestClient(Constants.PopcornApiEndpoint);
            var request = new RestRequest("/{segment}", Method.POST);
            request.AddUrlSegment("segment", "oauth/token");
            request.AddParameter("username", user.Username);
            request.AddParameter("password", user.Password);
            request.AddParameter("grant_type", "password");

            var response = await restClient.ExecutePostTaskAsync(request, ct);
            if (response.ErrorException != null)
            {
                throw new Exception();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {

            }

            var bearer =
                await Task.Run(() => JsonConvert.DeserializeObject<Bearer>(response.Content), ct);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"Signin ({user.Username}, {user.Fullname}, {user.Email}) in {elapsedMs} milliseconds.");

            return bearer;
        }

        #endregion

        #region Method -> ComputeMovieHistoryAsync

        /// <summary>
        /// Retrieve from database and set the IsFavorite and HasBeenSeen properties of each movie in params, 
        /// </summary>
        /// <param name="movies">All movies to compute</param>
        public async Task ComputeMovieHistoryAsync(IEnumerable<MovieShort> movies)
        {
            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.MovieHistory.LoadAsync();
                    var history = await context.MovieHistory.FirstOrDefaultAsync();
                    if (history == null)
                    {
                        await CreateMovieHistoryAsync();
                        history = await context.MovieHistory.FirstOrDefaultAsync();
                    }

                    foreach (var movie in movies)
                    {
                        var entityMovie = history.MoviesShort.FirstOrDefault(p => p.MovieId == movie.Id);
                        if (entityMovie != null)
                        {
                            movie.IsFavorite = entityMovie.IsFavorite;
                            movie.HasBeenSeen = entityMovie.HasBeenSeen;
                        }
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    $"ComputeMovieHistoryAsync in {elapsedMs} milliseconds.");
            });
        }

        #endregion

        #region Method -> GetFavoritesMoviesAsync

        /// <summary>
        /// Get the favorites movies
        /// </summary>
        /// <returns>Favorites movies</returns>
        public async Task<IEnumerable<MovieShort>> GetFavoritesMoviesAsync()
        {
            var watch = Stopwatch.StartNew();

            var movies = new List<MovieShort>();
            await Task.Run(async () =>
            {
                using (var context = new ApplicationDbContext())
                {
                    await context.MovieHistory.LoadAsync();
                    var movieHistory = await context.MovieHistory.FirstOrDefaultAsync();
                    foreach (var movie in movieHistory.MoviesShort.Where(p => p.IsFavorite))
                    {
                        movies.Add(MovieShortFromEntityToModel(movie));
                    }
                }
            });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"GetFavoritesMoviesIdAsync in {elapsedMs} milliseconds.");
            return movies;
        }

        #endregion

        #region Method -> GetSeenMoviesAsync

        /// <summary>
        /// Get the seen movies
        /// </summary>
        /// <returns>Seen movies</returns>
        public async Task<IEnumerable<MovieShort>> GetSeenMoviesAsync()
        {
            var watch = Stopwatch.StartNew();

            var movies = new List<MovieShort>();
            await Task.Run(async () =>
            {
                using (var context = new ApplicationDbContext())
                {
                    await context.MovieHistory.LoadAsync();
                    var movieHistory = await context.MovieHistory.FirstOrDefaultAsync();
                    foreach (var movie in movieHistory.MoviesShort.Where(p => p.HasBeenSeen))
                    {
                        movies.Add(MovieShortFromEntityToModel(movie));
                    }
                }
            });

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Debug(
                $"GetSeenMoviesIdAsync in {elapsedMs} milliseconds.");
            return movies;
        }

        #endregion

        #region Method -> SetFavoriteMovieAsync

        /// <summary>
        /// Set the movie as favorite
        /// </summary>
        /// <param name="movie">Favorite movie</param>
        public async Task SetFavoriteMovieAsync(MovieShort movie)
        {
            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.MovieHistory.LoadAsync();
                    var movieHistory = await context.MovieHistory.FirstOrDefaultAsync();
                    if (movieHistory == null)
                    {
                        await CreateMovieHistoryAsync();
                        movieHistory = await context.MovieHistory.FirstOrDefaultAsync();
                    }

                    if (movieHistory.MoviesShort == null)
                    {
                        movieHistory.MoviesShort = new List<Entity.Movie.MovieShort>
                        {
                            MovieShortFromModelToEntity(movie)
                        };

                        context.MovieHistory.AddOrUpdate(movieHistory);
                    }
                    else
                    {
                        var movieShort = movieHistory.MoviesShort.FirstOrDefault(p => p.MovieId == movie.Id);
                        if (movieShort == null)
                        {
                            movieHistory.MoviesShort.Add(MovieShortFromModelToEntity(movie));
                        }
                        else
                        {
                            movieShort.IsFavorite = movie.IsFavorite;
                        }
                    }

                    await context.SaveChangesAsync();
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    $"LikeMovieAsync ({movie.ImdbCode}) in {elapsedMs} milliseconds.");
            });
        }

        #endregion

        #region Method -> SetHasBeenSeenMovieAsync

        /// <summary>
        /// Set a movie as seen
        /// </summary>
        /// <param name="movie">Seen movie</param>
        public async Task SetHasBeenSeenMovieAsync(MovieFull movie)
        {
            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.MovieHistory.LoadAsync();
                    var movieHistory = await context.MovieHistory.FirstOrDefaultAsync();
                    if (movieHistory == null)
                    {
                        await CreateMovieHistoryAsync();
                        movieHistory = await context.MovieHistory.FirstOrDefaultAsync();
                    }

                    if (movieHistory.MoviesFull == null)
                    {
                        movieHistory.MoviesFull = new List<Entity.Movie.MovieFull>
                        {
                            MovieFullFromModelToEntity(movie)
                        };

                        context.MovieHistory.AddOrUpdate(movieHistory);
                    }
                    else
                    {
                        var movieFull = movieHistory.MoviesFull.FirstOrDefault(p => p.MovieId == movie.Id);
                        if (movieFull == null)
                        {
                            movieHistory.MoviesFull.Add(MovieFullFromModelToEntity(movie));
                        }
                        else
                        {
                            movieFull.HasBeenSeen = movie.HasBeenSeen;
                        }
                    }

                    await context.SaveChangesAsync();
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    $"SeenMovieAsync ({movie.ImdbCode}) in {elapsedMs} milliseconds.");
            });
        }

        #endregion

        #region Method -> CreateMovieHistoryAsync

        /// <summary>
        /// Scaffold UserData Table on database if empty
        /// </summary>
        private static async Task CreateMovieHistoryAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                var watch = Stopwatch.StartNew();

                await context.MovieHistory.LoadAsync();
                var userData = await context.MovieHistory.FirstOrDefaultAsync();
                if (userData == null)
                {
                    context.MovieHistory.AddOrUpdate(new MovieHistory
                    {
                        Created = DateTime.Now,
                        MoviesShort = new List<Entity.Movie.MovieShort>(),
                        MoviesFull = new List<Entity.Movie.MovieFull>()
                    });

                    await context.SaveChangesAsync();
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    $"CreateUserData in {elapsedMs} milliseconds.");
            }
        }

        #endregion

        #region Method -> MovieShortFromEntityToModel

        /// <summary>
        /// Convert a short movie entity to a short movie model
        /// </summary>
        /// <param name="movie">The movie to convert</param>
        /// <returns>Short movie model</returns>
        private MovieShort MovieShortFromEntityToModel(Entity.Movie.MovieShort movie)
        {
            var torrents = new List<Torrent>();
            foreach (var torrent in movie.Torrents)
            {
                torrents.Add(new Torrent
                {
                    DateUploaded = torrent.DateUploaded,
                    Url = torrent.Url,
                    Quality = torrent.Quality,
                    DateUploadedMix = torrent.DateUploadedMix,
                    Framerate = torrent.Framerate,
                    Hash = torrent.Hash,
                    Peers = torrent.Peers,
                    Resolution = torrent.Resolution,
                    Seeds = torrent.Seeds,
                    Size = torrent.Size,
                    SizeBytes = torrent.SizeBytes
                });
            }

            return new MovieShort
            {
                Language = movie.Language,
                ApiVersion = movie.ApiVersion,
                CoverImagePath = movie.CoverImagePath,
                DateUploaded = movie.DateUploaded,
                DateUploadedUnix = movie.DateUploadedUnix,
                ExecutionTime = movie.ExecutionTime,
                Genres = movie.Genres.Select(x => x.Name),
                HasBeenSeen = movie.HasBeenSeen,
                Id = movie.MovieId,
                ImdbCode = movie.ImdbCode,
                IsFavorite = movie.IsFavorite,
                Runtime = movie.Runtime,
                Rating = movie.Rating,
                MpaRating = movie.MpaRating,
                Title = movie.Title,
                TitleLong = movie.TitleLong,
                Torrents = torrents,
                MediumCoverImage = movie.MediumCoverImage,
                Url = movie.Url,
                State = movie.State,
                ServerTimezone = movie.ServerTimezone,
                ServerTime = movie.ServerTime,
                SmallCoverImage = movie.SmallCoverImage,
                Year = movie.Year
            };
        }

        #endregion

        #region Method -> MovieShortFromModelToEntity

        /// <summary>
        /// Convert a short movie model to a short movie entity
        /// </summary>
        /// <param name="movie">The movie to convert</param>
        /// <returns>Short movie entity</returns>
        private static Entity.Movie.MovieShort MovieShortFromModelToEntity(MovieShort movie)
        {
            var torrents = new List<Entity.Movie.Torrent>();
            foreach (var torrent in movie.Torrents)
            {
                torrents.Add(new Entity.Movie.Torrent
                {
                    DateUploaded = torrent.DateUploaded,
                    Url = torrent.Url,
                    Quality = torrent.Quality,
                    DateUploadedMix = torrent.DateUploadedMix,
                    Framerate = torrent.Framerate,
                    Hash = torrent.Hash,
                    Peers = torrent.Peers,
                    Resolution = torrent.Resolution,
                    Seeds = torrent.Seeds,
                    Size = torrent.Size,
                    SizeBytes = torrent.SizeBytes
                });
            }

            var genres = new List<Genre>();
            foreach (var genre in movie.Genres)
            {
                genres.Add(new Genre
                {
                    Name = genre
                });
            }

            var movieShort = new Entity.Movie.MovieShort
            {
                MovieId = movie.Id,
                IsFavorite = movie.IsFavorite,
                HasBeenSeen = movie.HasBeenSeen,
                ServerTime = movie.ServerTime,
                ServerTimezone = movie.ServerTimezone,
                SmallCoverImage = movie.SmallCoverImage,
                State = movie.State,
                Year = movie.Year,
                Language = movie.Language,
                ImdbCode = movie.ImdbCode,
                Title = movie.Title,
                Id = movie.Id,
                DateUploaded = movie.DateUploaded,
                Runtime = movie.Runtime,
                Url = movie.Url,
                TitleLong = movie.TitleLong,
                Torrents = torrents,
                MediumCoverImage = movie.MediumCoverImage,
                Genres = genres,
                DateUploadedUnix = movie.DateUploadedUnix,
                CoverImagePath = movie.CoverImagePath,
                MpaRating = movie.MpaRating,
                Rating = movie.Rating,
                ExecutionTime = movie.ExecutionTime,
                ApiVersion = movie.ApiVersion
            };
            return movieShort;
        }

        #endregion

        #region Method -> MovieShortFromModelToEntity

        /// <summary>
        /// Convert a full movie model to a full movie entity
        /// </summary>
        /// <param name="movie">The movie to convert</param>
        /// <returns>Full movie entity</returns>
        private static Entity.Movie.MovieFull MovieFullFromModelToEntity(MovieFull movie)
        {
            var torrents = new List<Entity.Movie.Torrent>();
            foreach (var torrent in movie.Torrents)
            {
                torrents.Add(new Entity.Movie.Torrent
                {
                    DateUploaded = torrent.DateUploaded,
                    Url = torrent.Url,
                    Quality = torrent.Quality,
                    DateUploadedMix = torrent.DateUploadedMix,
                    Framerate = torrent.Framerate,
                    Hash = torrent.Hash,
                    Peers = torrent.Peers,
                    Resolution = torrent.Resolution,
                    Seeds = torrent.Seeds,
                    Size = torrent.Size,
                    SizeBytes = torrent.SizeBytes
                });
            }

            var genres = new List<Genre>();
            foreach (var genre in movie.Genres)
            {
                genres.Add(new Genre
                {
                    Name = genre
                });
            }

            var images = new Images
            {
                BackgroundImage = movie.Images.BackgroundImage,
                MediumCoverImage = movie.Images.MediumCoverImage,
                SmallCoverImage = movie.Images.SmallCoverImage,
                LargeCoverImage = movie.Images.LargeCoverImage,
                LargeScreenshotImage1 = movie.Images.LargeScreenshotImage1,
                LargeScreenshotImage2 = movie.Images.LargeScreenshotImage2,
                LargeScreenshotImage3 = movie.Images.MediumScreenshotImage3,
                MediumScreenshotImage3 = movie.Images.MediumScreenshotImage3,
                MediumScreenshotImage1 = movie.Images.MediumScreenshotImage1,
                MediumScreenshotImage2 = movie.Images.MediumScreenshotImage2
            };

            var actors = new List<Actor>();
            foreach (var actor in movie.Actors)
            {
                actors.Add(new Actor
                {
                    CharacterName = actor.CharacterName,
                    MediumImage = actor.MediumImage,
                    Name = actor.Name,
                    SmallImage = actor.SmallImage,
                    SmallImagePath = actor.SmallImagePath
                });
            }

            var directors = new List<Director>();
            foreach (var actor in movie.Directors)
            {
                directors.Add(new Director
                {
                    MediumImage = actor.MediumImage,
                    Name = actor.Name,
                    SmallImage = actor.SmallImage,
                    SmallImagePath = actor.SmallImagePath
                });
            }

            var movieFull = new Entity.Movie.MovieFull
            {
                MovieId = movie.Id,
                Year = movie.Year,
                Language = movie.Language,
                ImdbCode = movie.ImdbCode,
                Title = movie.Title,
                Id = movie.Id,
                DateUploaded = movie.DateUploaded,
                Runtime = movie.Runtime,
                Url = movie.Url,
                TitleLong = movie.TitleLong,
                Torrents = torrents,
                Genres = genres,
                DateUploadedUnix = movie.DateUploadedUnix,
                MpaRating = movie.MpaRating,
                Rating = movie.Rating,
                Images = images,
                DescriptionFull = movie.DescriptionFull,
                Actors = actors,
                Directors = directors,
                DescriptionIntro = movie.DescriptionIntro,
                DownloadCount = movie.DownloadCount,
                LikeCount = movie.LikeCount,
                RtAudienceRating = movie.RtAudienceRating,
                RtAudienceScore = movie.RtAudienceScore,
                RtCriticsRating = movie.RtCriticsRating,
                RtCrtiticsScore = movie.RtCrtiticsScore,
                YtTrailerCode = movie.YtTrailerCode,
                HasBeenSeen = movie.HasBeenSeen,
                IsFavorite = movie.IsFavorite
            };
            return movieFull;
        }

        #endregion

        #endregion
    }
}