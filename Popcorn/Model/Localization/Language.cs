using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System.Diagnostics;
using NLog;
using Popcorn.Service.Language;

namespace Popcorn.Model.Localization
{
    /// <summary>
    /// Language
    /// </summary>
    public class Language : ObservableObject
    {
        #region Logger

        /// <summary>
        /// Logger of the class
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        #region Property -> LanguageService

        /// <summary>
        /// Service used to interacts with languages
        /// </summary>
        private ILanguageService LanguageService { get; }

        #endregion

        #region Property -> Languages

        private ICollection<ILanguage> _languages;

        /// <summary>
        /// Available languages of the application
        /// </summary>
        public ICollection<ILanguage> Languages
        {
            get { return _languages; }
            set { Set(() => Languages, ref _languages, value); }
        }

        #endregion

        #region Property -> CurrentLanguage

        private ILanguage _currentLanguages;

        /// <summary>
        /// Current language used in the application
        /// </summary>
        public ILanguage CurrentLanguage
        {
            get { return _currentLanguages; }
            set
            {
                Task.Run(async () =>
                {
                    await LanguageService.SetCurrentLanguageAsync(value);
                });
                Set(() => CurrentLanguage, ref _currentLanguages, value);
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Language()
        {
            LanguageService = SimpleIoc.Default.GetInstance<ILanguageService>();
            Task.Run(async () =>
            {
                await LoadLanguages();
            });
        }

        #endregion

        #region Methods

        #region Method -> LoadLanguages

        /// <summary>
        /// Load languages
        /// </summary>
        private async Task LoadLanguages()
        {
            var watchStart = Stopwatch.StartNew();

            CurrentLanguage = await LanguageService.GetCurrentLanguageAsync();
            Languages = await LanguageService.GetAvailableLanguagesAsync();

            watchStart.Stop();
            var elapsedLanguageMs = watchStart.ElapsedMilliseconds;
            Logger.Info(
                "Languages loaded in {0} milliseconds.", elapsedLanguageMs);
        }

        #endregion

        #endregion

        #endregion
    }
}