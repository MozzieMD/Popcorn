namespace Popcorn.ViewModel.Tabs
{
    /// <summary>
    /// Interface used to describe a tab of the UI tabcontrol
    /// </summary>
    public interface ITab
    {
        #region Properties

        #region Property -> TabName
        /// <summary>
        /// Tab's name
        /// </summary>
        string TabName { get; set; }
        #endregion

        #endregion
    }
}
