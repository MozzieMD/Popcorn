using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Popcorn.Model.Account;
using Popcorn.Model.Movie;

namespace Popcorn.Service.User
{
    /// <summary>
    /// Interface used to describe a user service
    /// </summary>
    public interface IUserDataService
    {
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
        Task<Model.Account.User> CreateUser(string username, string firstname, string lastname,
            string password, string email, CancellationToken ct);

        /// <summary>
        /// Signin with a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>Bearer</returns>
        Task<Bearer> Signin(Model.Account.User user, CancellationToken ct);

        /// <summary>
        /// Retrieve movies history (movie likes and seen)
        /// </summary>
        /// <param name="movies">Movies to process</param>
        /// <returns></returns>
        Task ComputeMovieHistoryAsync(IEnumerable<MovieShort> movies);

        /// <summary>
        /// Record in database that the movie has been liked
        /// </summary>
        /// <param name="movie">Movie to process</param>
        /// <returns></returns>
        Task LikeMovieAsync(MovieShort movie);

        /// <summary>
        /// Record in database that the movie has been seen
        /// </summary>
        /// <param name="movie">Movie to process</param>
        /// <returns></returns>
        Task SeenMovieAsync(MovieFull movie);
    }
}
