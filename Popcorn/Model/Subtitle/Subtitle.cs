using Popcorn.Model.Localization;
using Popcorn.Model.Subtitle.Json;

namespace Popcorn.Model.Subtitle
{
    public class Subtitle : SubtitleDeserialized
    {
        #region Properties

        #region Property -> Language
        /// <summary>
        /// Language's subtitle
        /// </summary>
        private ILanguage _language;
        /// <summary>
        /// Language's subtitle
        /// </summary>
        public ILanguage Language
        {
            get { return _language; }
            set { Set(() => Language, ref _language, value); }
        }
        #endregion

        #endregion
    }
}
