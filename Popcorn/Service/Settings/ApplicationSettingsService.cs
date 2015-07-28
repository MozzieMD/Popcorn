using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using Popcorn.Entity;
using Popcorn.Entity.Settings;
using NLog;
using System.Threading.Tasks;

namespace Popcorn.Service.Settings
{
    /// <summary>
    /// Service used to interacts with application's settings
    /// </summary>
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        #region Logger
        /// <summary>
        /// Logger of the class
        /// </summary>
        private readonly static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Methods

        #region Method -> CreateApplicationSettingsAsync
        /// <summary>
        /// Scaffold ApplicationSettings table on database if empty
        /// </summary>
        public async Task CreateApplicationSettingsAsync()
        {
            var watch = Stopwatch.StartNew();

            var englishLanguage = new Entity.Localization.Language
            {
                LocalizedName = "English",
                EnglishName = "English",
                Culture = "en",
                IsCurrentLanguage = true
            };

            var frenchLanguage = new Entity.Localization.Language
            {
                LocalizedName = "Français",
                EnglishName = "French",
                Culture = "fr",
                IsCurrentLanguage = false
            };

            using (var context = new ApplicationDbContext())
            {
                await context.ApplicationSettings.LoadAsync();
                var settings = await context.ApplicationSettings?.FirstOrDefaultAsync();
                if (settings == null)
                {
                    context.ApplicationSettings.AddOrUpdate(new ApplicationSettings
                    {
                        Version = Helpers.Constants.ApplicationVersion,
                        Languages = new List<Entity.Localization.Language>
                            {
                                englishLanguage,
                                frenchLanguage
                            }
                    });
                }
                else if (settings.Languages == null)
                {
                    settings.Languages = new List<Entity.Localization.Language>
                        {
                            englishLanguage,
                            frenchLanguage
                        };
                }

                await context.SaveChangesAsync();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Debug(
                "CreateApplicationSettings in {0} milliseconds.", elapsedMs);
        }
        #endregion

        #endregion
    }
}
