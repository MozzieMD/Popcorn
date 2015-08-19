using GalaSoft.MvvmLight.Messaging;
using Popcorn.Models.Movie;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast the movie whose has been seen value has changed
    /// </summary>
    public class ChangeHasBeenSeenMovieMessage : MessageBase
    {
        #region Properties

        #region Property -> Movie

        /// <summary>
        /// Movie
        /// </summary>
        public readonly MovieFull Movie;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="movie">The movie</param>
        public ChangeHasBeenSeenMovieMessage(MovieFull movie)
        {
            Movie = movie;
        }

        #endregion
    }
}