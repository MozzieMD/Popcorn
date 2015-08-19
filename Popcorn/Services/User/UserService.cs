using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Popcorn.Helpers;
using Popcorn.Models.Account;
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

        #endregion
    }
}