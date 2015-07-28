using System.Collections.Generic;
using System.Threading.Tasks;
using Popcorn.Model.Movie;

namespace Popcorn.Service.User
{
    /// <summary>
    /// Interface used to describe a user service
    /// </summary>
    public interface IUserDataService
    {
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
