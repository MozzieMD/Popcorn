using GalaSoft.MvvmLight;
using Popcorn.Model.Localization;

namespace Popcorn.ViewModel.Settings
{
    /// <summary>
    /// Application's settings
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> Language

        private Language _language;

        /// <summary>
        /// Language
        /// </summary>
        public Language Language
        {
            get { return _language; }
            private set { Set(() => Language, ref _language, value); }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SettingsViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            Language = new Language();
        }

        #endregion
    }
}