using GalaSoft.MvvmLight.Messaging;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast the search of a movie
    /// </summary>
    public class SearchMovieMessage : MessageBase
    {
        #region Properties

        #region Property -> Filter
        /// <summary>
        /// Movie
        /// </summary>
        public readonly string Filter;

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filter">Filter use as criteria for search</param>
        public SearchMovieMessage(string filter)
        {
            Filter = filter;
        }
        #endregion
    }
}
