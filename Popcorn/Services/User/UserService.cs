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
using Popcorn.Entity.User;
using Popcorn.Helpers;
using Popcorn.Models.Account;
using Popcorn.Models.Movie;
using RestSharp;

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