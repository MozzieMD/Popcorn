using GalaSoft.MvvmLight.Messaging;
using Popcorn.Models.Movie;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast the movie whose favorite value has changed
    /// </summary>
    public class ChangeFavoriteMovieMessage : MessageBase
    {
        #region Properties

        #region Property -> Movie

        /// <summary>
        /// Movie
        /// </summary>
        public readonly MovieShort Movie;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="movie">The movie</param>
        public ChangeFavoriteMovieMessage(MovieShort movie)
        {
            Movie = movie;
        }

        #endregion
    }
}