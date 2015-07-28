using GalaSoft.MvvmLight.Messaging;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast a screen mode change
    /// </summary>
    public class ChangeScreenModeMessage : MessageBase
    {
        #region Properties

        #region Property -> IsFullScreen
        /// <summary>
        /// Indicates if the new screen mode is fullscreen
        /// </summary>
        public bool IsFullScreen { get; private set; }
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isFullScreen">Indicates if the new screen mode is fullscreen</param>
        public ChangeScreenModeMessage(bool isFullScreen)
        {
            IsFullScreen = isFullScreen;
        }
        #endregion
    }
}
