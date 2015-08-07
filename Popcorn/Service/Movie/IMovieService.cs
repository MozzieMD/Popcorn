using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using Popcorn.Model.Localization;
using Popcorn.Model.Movie;

namespace Popcorn.Service.Movie
{
    /// <summary>
    /// Interface used to describe a service to interact with movies
    /// </summary>
    public interface IMovieService
    {
        /// <summary>
        /// Get popular movies by page
        /// </summary>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Popular movies</returns>
        Task<IEnumerable<MovieShort>> GetPopularMoviesAsync(int page,
            int limit,
            CancellationToken ct);

        /// <summary>
        /// Get top rated movies by page
        /// </summary>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Top rated movies</returns>
        Task<IEnumerable<MovieShort>> GetTopRatedMoviesAsync(int page,
            int limit,
            CancellationToken ct);

        /// <summary>
        /// Get recent movies by page
        /// </summary>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Recent movies</returns>
        Task<IEnumerable<MovieShort>> GetRecentMoviesAsync(int page,
            int limit,
            CancellationToken ct);

        /// <summary>
        /// Search movies by criteria
        /// </summary>
        /// <param name="search">Criteria used for search</param>
        /// <param name="page">Page to return</param>
        /// <param name="limit">The maximum number of movies to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Searched movies</returns>
        Task<IEnumerable<MovieShort>> SearchMoviesAsync(string search,
            int page,
            int limit,
            CancellationToken ct);

        /// <summary>
        /// Get TMDb movie informations
        /// </summary>
        /// <param name="movie">Movie to load</param>
        /// <returns>Movie's full details</returns>
        Task<MovieFull> GetMovieFullDetailsAsync(MovieShort movie);

        /// <summary>
        /// Translate movie informations (title, description, ...)
        /// </summary>
        /// <param name="movie">Movie to translate</param>
        /// <returns>Task</returns>
        Task TranslateMovieShortAsync(MovieShort movie);

        /// <summary>
        /// Translate movie informations (title, description, ...)
        /// </summary>
        /// <param name="movie">Movie to translate</param>
        /// <returns>Task</returns>
        Task TranslateMovieFullAsync(MovieFull movie);

        /// <summary>
        /// Get the movie's subtitles
        /// </summary>
        /// <param name="movie">The movie of which to retrieve its subtitles</param>
        /// <param name="ct">Cancellation token</param>
        Task LoadSubtitlesAsync(MovieFull movie,
            CancellationToken ct);

        /// <summary>
        /// Download a subtitle
        /// </summary>
        /// <param name="movie">The movie of which to retrieve its subtitles</param>
        Task DownloadSubtitleAsync(MovieFull movie);

        /// <summary>
        /// Get the link to the youtube trailer of a movie
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <returns>Video trailer</returns>
        Task<ResultContainer<Video>> GetMovieTrailerAsync(MovieFull movie);

        /// <summary>
        /// Download the movie's background image
        /// </summary>
        /// <param name="movie">The movie to process</param>
        Task DownloadBackgroundImageAsync(MovieFull movie);

        /// <summary>
        /// Download cover image for each of the movies provided
        /// </summary>
        /// <param name="movies">The movies to process</param>
        Task DownloadCoverImageAsync(IEnumerable<MovieShort> movies);

        /// <summary>
        /// Download the movie's poster image
        /// </summary>
        /// <param name="movie">The movie to process</param>
        Task DownloadPosterImageAsync(MovieFull movie);

        /// <summary>
        /// Download directors' image for a movie
        /// </summary>
        /// <param name="movie">The movie to process</param>
        Task DownloadDirectorImageAsync(MovieFull movie);

        /// <summary>
        /// Download actors' image for a movie
        /// </summary>
        /// <param name="movie">The movie to process</param>
        Task DownloadActorImageAsync(MovieFull movie);

        /// <summary>
        /// Change the culture of TMDb
        /// </summary>
        /// <param name="language">Language to set</param>
        void ChangeTmdbLanguage(ILanguage language);
    }
}