using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Popcorn.Entity;
using Popcorn.Entity.User;
using Popcorn.Model.Movie;

namespace Popcorn.Service.User
{
    /// <summary>
    /// Service used to interacts with user's data
    /// </summary>
    public class UserDataService : IUserDataService
    {
        #region Logger

        /// <summary>
        /// Logger of the class
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        #region Method -> ComputeMovieHistoryAsync

        /// <summary>
        /// Retrieve from database and set the IsLiked and IsSeen properties of each movie in params, 
        /// </summary>
        /// <param name="movies">All movies to compute</param>
        public async Task ComputeMovieHistoryAsync(IEnumerable<MovieShort> movies)
        {
            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.UserData.LoadAsync();
                    var userData = await context.UserData.FirstOrDefaultAsync();
                    if (userData == null)
                    {
                        await CreateUserDataAsync();
                        userData = await context.UserData.FirstOrDefaultAsync();
                    }

                    foreach (var movie in movies)
                    {
                        var movieHistory = userData?.MovieHistory?.FirstOrDefault(p => p.ImdbCode == movie.ImdbCode);
                        movie.IsLiked = movieHistory?.Liked;
                        movie.IsSeen = movieHistory?.Seen;
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    "ComputeMovieHistoryAsync in {0} milliseconds.", elapsedMs);
            });
        }

        #endregion

        #region Method -> LikeMovieAsync

        /// <summary>
        /// Set the Liked database field when a movie has been liked
        /// </summary>
        /// <param name="movie">Liked movie</param>
        public async Task LikeMovieAsync(MovieShort movie)
        {
            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.UserData.LoadAsync();
                    var userData = await context.UserData.FirstOrDefaultAsync();
                    if (userData == null)
                    {
                        await CreateUserDataAsync();
                        userData = await context.UserData.FirstOrDefaultAsync();
                    }

                    if (userData.MovieHistory == null)
                    {
                        userData.MovieHistory = new List<MovieHistory>
                        {
                            new MovieHistory
                            {
                                ImdbCode = movie.ImdbCode,
                                Liked = true,
                                Seen = false
                            }
                        };
                        movie.IsLiked = true;
                        context.UserData.AddOrUpdate(userData);
                    }
                    else
                    {
                        var movieHistory = userData.MovieHistory?.FirstOrDefault(p => p.ImdbCode == movie.ImdbCode);
                        if (movieHistory == null)
                        {
                            userData.MovieHistory.Add(new MovieHistory
                            {
                                ImdbCode = movie.ImdbCode,
                                Liked = true,
                                Seen = false
                            });
                            movie.IsLiked = true;
                        }
                        else
                        {
                            movieHistory.Liked = !movieHistory.Liked;
                            movie.IsLiked = movieHistory.Liked;
                        }
                    }

                    await context.SaveChangesAsync();
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    "LikeMovieAsync ({0}) in {1} milliseconds.", movie.ImdbCode, elapsedMs);
            });
        }

        #endregion

        #region Method -> SeenMovieAsync

        /// <summary>
        /// Set the Seen database field when a movie has been seen
        /// </summary>
        /// <param name="movie">Seen movie</param>
        public async Task SeenMovieAsync(MovieFull movie)
        {
            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.UserData.LoadAsync();
                    var userData = await context.UserData.FirstOrDefaultAsync();
                    if (userData == null)
                    {
                        await CreateUserDataAsync();
                        userData = await context.UserData.FirstOrDefaultAsync();
                    }
                    
                    if (userData.MovieHistory == null)
                    {
                        userData.MovieHistory = new List<MovieHistory>
                        {
                            new MovieHistory
                            {
                                ImdbCode = movie.ImdbCode,
                                Liked = false,
                                Seen = true
                            }
                        };

                        context.UserData.AddOrUpdate(userData);
                    }
                    else
                    {
                        var movieHistory = userData.MovieHistory?.FirstOrDefault(p => p.ImdbCode == movie.ImdbCode);
                        if (movieHistory == null)
                        {
                            userData.MovieHistory.Add(new MovieHistory
                            {
                                ImdbCode = movie.ImdbCode,
                                Liked = false,
                                Seen = true
                            });
                        }
                        else
                        {
                            movieHistory.Seen = true;
                        }
                    }

                    await context.SaveChangesAsync();
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    "SeenMovieAsync ({0}) in {1} milliseconds.", movie.ImdbCode, elapsedMs);
            });
        }

        #endregion

        #region Method -> CreateUserDataAsync

        /// <summary>
        /// Scaffold UserData Table on database if empty
        /// </summary>
        private static async Task CreateUserDataAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                var watch = Stopwatch.StartNew();

                await context.UserData.LoadAsync();
                var userData = await context.UserData.FirstOrDefaultAsync();
                if (userData == null)
                {
                    context.UserData.AddOrUpdate(new UserData
                    {
                        UserName = "Default",
                        MovieHistory = new List<MovieHistory>()
                    });

                    await context.SaveChangesAsync();
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    "CreateUserData in {0} milliseconds.", elapsedMs);
            }
        }

        #endregion

        #endregion
    }
}