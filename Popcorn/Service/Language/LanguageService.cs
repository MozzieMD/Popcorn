using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WPFLocalizeExtension.Engine;
using Popcorn.Entity;
using Popcorn.Messaging;
using Popcorn.Model.Localization;
using Popcorn.Service.Movie;
using Popcorn.Service.Settings;

namespace Popcorn.Service.Language
{
    /// <summary>
    /// Service used to interacts with languages
    /// </summary>
    public class LanguageService : ILanguageService
    {
        #region Logger

        /// <summary>
        /// Logger of the class
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Property -> ApplicationService

        /// <summary>
        /// Service used to interacts with application settings
        /// </summary>
        private IApplicationSettingsService ApplicationService { get; }

        #endregion

        #region Property -> MovieService

        /// <summary>
        /// Service used to interact with movies
        /// </summary>
        private IMovieService MovieService { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LanguageService()
        {
            ApplicationService = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();
            MovieService = SimpleIoc.Default.GetInstance<IMovieService>();
        }

        #endregion

        #region Methods

        #region Method -> GetAvailableLanguagesAsync

        /// <summary>
        /// Get all available languages from the database
        /// </summary>
        /// <returns>All available languages</returns>
        public async Task<ICollection<ILanguage>> GetAvailableLanguagesAsync()
        {
            ICollection<ILanguage> availableLanguages = null;
            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.ApplicationSettings.LoadAsync();
                    var applicationSettings = await context.ApplicationSettings.FirstOrDefaultAsync();
                    if (applicationSettings == null)
                    {
                        await ApplicationService.CreateApplicationSettingsAsync();
                        applicationSettings = await context.ApplicationSettings.FirstOrDefaultAsync();
                    }
                    var languages = applicationSettings.Languages;
                    availableLanguages = new List<ILanguage>();
                    foreach (var language in languages)
                    {
                        switch (language.Culture)
                        {
                            case "en":
                                availableLanguages.Add(new EnglishLanguage());
                                break;
                            case "fr":
                                availableLanguages.Add(new FrenchLanguage());
                                break;
                            default:
                                availableLanguages.Add(new EnglishLanguage());
                                break;
                        }
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    "GetAvailableLanguagesAsync in {0} milliseconds.", elapsedMs);
            });

            return availableLanguages;
        }

        #endregion

        #region Method -> GetCurrentLanguageAsync

        /// <summary>
        /// Get the current language of the application
        /// </summary>
        /// <returns>Current language</returns>
        public async Task<ILanguage> GetCurrentLanguageAsync()
        {
            ILanguage currentLanguage = null;

            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.ApplicationSettings.LoadAsync();
                    var applicationSettings = await context.ApplicationSettings.FirstOrDefaultAsync();
                    if (applicationSettings == null)
                    {
                        await ApplicationService.CreateApplicationSettingsAsync();
                        applicationSettings = await context.ApplicationSettings.FirstOrDefaultAsync();
                    }
                    var language = applicationSettings.Languages.FirstOrDefault(a => a.IsCurrentLanguage);
                    if (language != null)
                    {
                        switch (language.Culture)
                        {
                            case "en":
                                currentLanguage = new EnglishLanguage();
                                break;
                            case "fr":
                                currentLanguage = new FrenchLanguage();
                                break;
                            default:
                                currentLanguage = new EnglishLanguage();
                                break;
                        }
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    "GetCurrentLanguageAsync in {0} milliseconds.", elapsedMs);
            });

            return currentLanguage;
        }

        #endregion

        #region Method -> SetCurrentLanguageAsync

        /// <summary>
        /// Get the current language of the application
        /// </summary>
        /// <param name="language">Language to set</param>
        public async Task SetCurrentLanguageAsync(ILanguage language)
        {
            await Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();

                using (var context = new ApplicationDbContext())
                {
                    await context.ApplicationSettings.LoadAsync();
                    var applicationSettings = await context.ApplicationSettings.FirstOrDefaultAsync();
                    if (applicationSettings == null)
                    {
                        await ApplicationService.CreateApplicationSettingsAsync();
                        applicationSettings = await context.ApplicationSettings.FirstOrDefaultAsync();
                    }
                    var currentLanguage = applicationSettings.Languages.First(a => a.Culture == language.Culture);
                    currentLanguage.IsCurrentLanguage = true;
                    foreach (var lang in applicationSettings.Languages.Where(a => a.Culture != language.Culture))
                    {
                        lang.IsCurrentLanguage = false;
                    }
                    context.ApplicationSettings.AddOrUpdate(applicationSettings);
                    await context.SaveChangesAsync();

                    ChangeLanguage(language);
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Logger.Debug(
                    "SetCurrentLanguageAsync ({0}) in {1} milliseconds.", language.LocalizedName, elapsedMs);
            });
        }

        #endregion

        #region Method -> ChangeLanguage

        /// <summary>
        /// Change language 
        /// </summary>
        /// <param name="language"></param>
        private void ChangeLanguage(ILanguage language)
        {
            MovieService.ChangeTmdbLanguage(language);
            LocalizeDictionary.Instance.Culture = new CultureInfo(language.Culture);
            Messenger.Default.Send(new ChangeLanguageMessage(language));
        }

        #endregion

        #endregion
    }
}