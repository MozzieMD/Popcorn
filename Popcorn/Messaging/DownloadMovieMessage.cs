using GalaSoft.MvvmLight.Messaging;
using Popcorn.Model.Movie;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast a downloading movie message action
    /// </summary>
    public class DownloadMovieMessage : MessageBase
    {
        #region Properties

        #region Property -> Movie
        /// <summary>
        /// New language
        /// </summary>
        public MovieFull Movie { get; private set; }

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// DownloadMovieMessage
        /// </summary>
        /// <param name="movie">The movie to download</param>
        public DownloadMovieMessage(MovieFull movie)
        {
            Movie = movie;
        }
        #endregion
    }
}
