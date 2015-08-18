namespace Popcorn.ViewModels.Tabs
{
    /// <summary>
    /// Interface used to describe a tab of the UI tabcontrol
    /// </summary>
    public interface ITab
    {
        #region Properties

        #region Property -> CurrentNumberOfMovies
        /// <summary>
        /// The current number of movies in the tab
        /// </summary>
        int CurrentNumberOfMovies { get; set; }
        #endregion

        #region Property -> MaxNumberOfMovies
        /// <summary>
        /// The maximum number of movies found
        /// </summary>
        int MaxNumberOfMovies { get; set; }
        #endregion

        #region Property -> TabName
        /// <summary>
        /// Tab's name
        /// </summary>
        string TabName { get; set; }
        #endregion

        #endregion
    }
}
