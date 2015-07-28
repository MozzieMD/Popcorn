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

        /// <summary>
        /// Language
        /// </summary>
        private Language _language;

        public Language Language
        {
            get { return _language; }
            private set { Set(() => Language, ref _language, value); }
        }

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// SettingsViewModel
        /// </summary>
        public SettingsViewModel()
        {
            Language = new Language();
        }
        #endregion
    }
}
