using GalaSoft.MvvmLight.Messaging;
using Popcorn.Model.Movie;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast the loading of a movie
    /// </summary>
    public class LoadMovieMessage : MessageBase
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
        /// <param name="movie">Movie</param>
        public LoadMovieMessage(MovieShort movie)
        {
            Movie = movie;
        }

        #endregion
    }
}