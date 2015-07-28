using System.IO;

namespace Popcorn.Helpers
{
    /// <summary>
    /// Constants of the project
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Directory of covers images
        /// </summary>
        public static readonly string CoverMoviesDirectory = Path.GetTempPath() + "Popcorn\\Covers\\";

        /// <summary>
        /// Directory of poster images
        /// </summary>
        public static readonly string PosterMovieDirectory = Path.GetTempPath() + "Popcorn\\Posters\\";

        /// <summary>
        /// Directory of background images 
        /// </summary>
        public static readonly string BackgroundMovieDirectory = Path.GetTempPath() + "Popcorn\\Backgrounds\\";

        /// <summary>
        /// Directory of directors images
        /// </summary>
        public static readonly string DirectorMovieDirectory = Path.GetTempPath() + "Popcorn\\Directors\\";

        /// <summary>
        /// Directory of actors images
        /// </summary>
        public static readonly string ActorMovieDirectory = Path.GetTempPath() + "Popcorn\\Actors\\";

        /// <summary>
        /// Directory of downloaded movies
        /// </summary>
        public static readonly string MovieDownloads = Path.GetTempPath() + "Popcorn\\Downloads\\";

        /// <summary>
        /// Subtitles directory
        /// </summary>
        public static readonly string Subtitles = Path.GetTempPath() + "Popcorn\\Subtitles\\";

        /// <summary>
        /// Logging directory
        /// </summary>
        public static readonly string Logging = Path.GetTempPath() + "Popcorn\\Logs\\";

        /// <summary>
        /// Endpoint to YTS
        /// </summary>
        public const string YtsApiEndpoint = "http://yts.to/api/v2";

        /// <summary>
        /// Endpoint to Yify Subtitles
        /// </summary>
        public const string YifySubtitles = "http://yifysubtitles.com";

        /// <summary>
        /// Endpoint to Yify Subtitles API
        /// </summary>
        public const string YifySubtitlesApi = "http://api.yifysubtitles.com/subs/";

        /// <summary>
        /// Client ID for TMDb
        /// </summary>
        public const string TmDbClientId = "a21fe922d3bac6654e93450e9a18af1c";

        /// <summary>
        /// Background image size for movie, retrieved from TMDb
        /// </summary>
        public const string BackgroundImageSizeTmDb = "original";

        /// <summary>
        /// Generic path to youtube video
        /// </summary>
        public const string YoutubePath = "http://www.youtube.com/watch?v=";

        /// <summary>
        /// We want at least 5 rows to be able to scroll inside the main window
        /// </summary>
        public const int NumberOfRowsPerPage = 5;

        /// <summary>
        /// In percentage, the minimum of buffering before we can actually start playing the movie
        /// </summary>
        public const double MinimumBufferingBeforeMoviePlaying = 2.0;

        /// <summary>
        /// The maximum number of movies per page to load from the API
        /// </summary>
        public const int MaxMoviesPerPage = 20;

        /// <summary>
        /// Extension of image file
        /// </summary>
        public const string ImageFileExtension = ".jpg";

        /// <summary>
        /// Extension of video file
        /// </summary>
        public const string VideoFileExtension = ".mp4";

        /// <summary>
        /// Version of the application
        /// </summary>
        public const string ApplicationVersion = "0.0.2";

        /// <summary>
        /// Url of the server updates
        /// </summary>
        public const string UpdateServerUrl = "https://popcorn.blob.core.windows.net/releases";

        /// <summary>
        /// Name of the application
        /// </summary>
        public const string ApplicationName = "Popcorn";

        public enum YoutubeStreamingQuality
        {
            Low = 0,
            Medium = 1,
            High = 2
        }
    }
}
