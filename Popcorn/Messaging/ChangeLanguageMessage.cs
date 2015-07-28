using GalaSoft.MvvmLight.Messaging;
using Popcorn.Model.Localization;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast a language change
    /// </summary>
    public class ChangeLanguageMessage : MessageBase
    {
        #region Properties

        #region Property -> Language
        /// <summary>
        /// New language
        /// </summary>
        public ILanguage Language { get; private set; }

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">New language</param>
        public ChangeLanguageMessage(ILanguage language)
        {
            Language = language;
        }
        #endregion
    }
}
