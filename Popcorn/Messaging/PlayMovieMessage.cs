using GalaSoft.MvvmLight.Messaging;
using Popcorn.Models.Movie;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to play a movie
    /// </summary>
    public class PlayMovieMessage : MessageBase
    {
        #region Properties

        #region Property -> Movie

        /// <summary>
        /// The buffered movie
        /// </summary>
        public readonly MovieFull Movie;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="movie">The movie</param>
        public PlayMovieMessage(MovieFull movie)
        {
            Movie = movie;
        }

        #endregion
    }
}