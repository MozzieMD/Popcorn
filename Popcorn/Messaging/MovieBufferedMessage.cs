using System;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Model.Movie;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast when a movie is buffered
    /// </summary>
    public class MovieBufferedMessage : MessageBase
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
        /// <param name="movie">The buffered movie</param>
        public MovieBufferedMessage(MovieFull movie)
        {
            Movie = movie;
        }

        #endregion
    }
}