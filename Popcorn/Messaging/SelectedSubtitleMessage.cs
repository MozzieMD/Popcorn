using GalaSoft.MvvmLight.Messaging;
using Popcorn.Model.Subtitle;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast a selected subtitle message
    /// </summary>
    public class SelectedSubtitleMessage : MessageBase
    {
        #region Properties

        #region Property -> Subtitle
        /// <summary>
        /// New language
        /// </summary>
        public Subtitle Subtitle { get; private set; }

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subtitle">The subtitle</param>
        public SelectedSubtitleMessage(Subtitle subtitle)
        {
            Subtitle = subtitle;
        }
        #endregion
    }
}
