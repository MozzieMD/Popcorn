using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using Popcorn.Entity;
using NLog;
using System.Threading.Tasks;

namespace Popcorn.Services.Settings
{
    /// <summary>
    /// Services used to interacts with application's settings
    /// </summary>
    public class ApplicationSettingsService
    {
        #region Logger

        /// <summary>
        /// Logger of the class
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        #region Method -> CreateApplicationSettingsAsync

        /// <summary>
        /// Scaffold Settings table on database if empty
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
                await context.Settings.LoadAsync();
                var settings = await context.Settings.FirstOrDefaultAsync();
                if (settings == null)
                {
                    context.Settings.AddOrUpdate(new Entity.Application.Settings
                    {
                        CreatedOn = DateTime.Now,
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
            Logger.Debug(
                "CreateApplicationSettings in {0} milliseconds.", elapsedMs);
        }

        #endregion

        #endregion
    }
}