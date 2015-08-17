using GalaSoft.MvvmLight;

namespace Popcorn.Models.Localization
{
    /// <summary>
    /// Custom language which is not yet represented into the application
    /// </summary>
    public sealed class CustomLanguage : ObservableObject, ILanguage
    {
        #region Properties

        #region Property -> LocalizedName

        /// <summary>
        /// Language's name
        /// </summary>
        public string LocalizedName { get; set; }

        #endregion

        #region Property -> EnglishName

        /// <summary>
        /// English language's name
        /// </summary>
        public string EnglishName { get; set; }

        #endregion

        #region Property -> Culture

        /// <summary>
        /// Language's culture
        /// </summary>
        public string Culture { get; set; }

        #endregion

        #endregion
    }
}