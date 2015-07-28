using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using Popcorn.Model.Localization;
using Popcorn.Model.Movie;
using Popcorn.Model.Subtitle;

namespace Popcorn.Service.Api
{
    /// <summary>
    /// Interface used to describe an API service
    /// </summary>
    public interface IApiService
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
        Task<ObservableCollection<Subtitle>> GetSubtitlesAsync(MovieFull movie,
            CancellationToken ct);

        /// <summary>
        /// Download a subtitle
        /// </summary>
        /// <param name="movie">The movie of which to retrieve its subtitles</param>
        /// <param name="subtitle">The movie's subtitle to retrieve</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Uri to the downloaded subtitle</returns>
        Task<Uri> DownloadSubtitleAsync(MovieFull movie,
            Subtitle subtitle,
            CancellationToken ct);

        /// <summary>
        /// Get the link to the youtube trailer of a movie
        /// </summary>
        /// <param name="movie">The movie</param>
        /// <returns>Video trailer</returns>
        Task<ResultContainer<Video>> GetMovieTrailerAsync(MovieFull movie);

        /// <summary>
        /// Download the movie's background image
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded background image</returns>
        Task<string> DownloadBackgroundImageAsync(string imdbCode,
            CancellationToken ct);

        /// <summary>
        /// Download the movie's cover image
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="uri">Resource's uri</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded cover image</returns>
        Task<string> DownloadCoverImageAsync(string imdbCode,
            Uri uri,
            CancellationToken ct);

        /// <summary>
        /// Download the movie's poster image
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="uri">Resource's uri</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded poster image</returns>
        Task<string> DownloadPosterImageAsync(string imdbCode,
            Uri uri,
            CancellationToken ct);

        /// <summary>
        /// Download the director's image profile
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="uri">Resource's uri</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded director's image</returns>
        Task<string> DownloadDirectorImageAsync(string imdbCode,
            Uri uri,
            CancellationToken ct);

        /// <summary>
        /// Download the actor's image profile
        /// </summary>
        /// <param name="imdbCode">Movie's Imdb code</param>
        /// <param name="uri">Resource's uri</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Local path to the downloaded actor's image</returns>
        Task<string> DownloadActorImageAsync(string imdbCode,
            Uri uri,
            CancellationToken ct);

        /// <summary>
        /// Change the culture of TMDb
        /// </summary>
        /// <param name="language">Language to set</param>
        void ChangeTmdbLanguage(ILanguage language);
    }
}
