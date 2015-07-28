using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using NLog;
using Squirrel;
using WPFLocalizeExtension.Engine;

namespace Popcorn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region Logger
        /// <summary>
        /// Logger of the class
        /// </summary>
        private readonly static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        private static readonly UpdateManager UpdateManager;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        static App()
        {
            var watchStart = Stopwatch.StartNew();

            logger.Info(
                "Popcorn starting...");

            AppDomain.CurrentDomain.ProcessExit += (sender, args) => UpdateManager.Dispose();

            Directory.CreateDirectory(Helpers.Constants.Logging);

            DispatcherHelper.Initialize();

            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;

            UpdateManager = new UpdateManager(Helpers.Constants.UpdateServerUrl, Helpers.Constants.ApplicationName);

            watchStart.Stop();
            var elapsedStartMs = watchStart.ElapsedMilliseconds;
            logger.Info(
                "Popcorn started in {0} milliseconds.", elapsedStartMs);

            Task.Run(async () =>
            {
                await StartUpdateProcessAsync();
            });
        }
        #endregion

        #region Methods

        #region Method -> StartUpdateProcessAsync
        /// <summary>
        /// Look for update then download and apply if any
        /// </summary>
        /// <returns></returns>
        private static async Task StartUpdateProcessAsync()
        {
            var watchStart = Stopwatch.StartNew();

            logger.Info(
                "Looking for updates...");
            try
            {
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v => UpdateManager.CreateShortcutForThisExe(),
                    onAppUpdate: v => UpdateManager.CreateShortcutForThisExe(),
                    onAppUninstall: v => UpdateManager.RemoveShortcutForThisExe());

                var updateInfo = await UpdateManager.CheckForUpdate();
                if (updateInfo == null)
                {
                    logger.Error(
                        "Problem while trying to check new updates.");
                    return;
                }

                if (updateInfo.ReleasesToApply.Any())
                {
                    logger.Info(
                        "A new update has been found!\nCurrently installed version: {0}\nNew update: {1}",
                        updateInfo.CurrentlyInstalledVersion?.Version?.Build,
                        updateInfo.FutureReleaseEntry?.Version?.Build);

                    await UpdateManager.DownloadReleases(updateInfo.ReleasesToApply, x =>
                    {
                        logger.Info(
                            "Downloading new update... {0}%", x);
                    });

                    await UpdateManager.ApplyReleases(updateInfo, x =>
                    {
                        logger.Info(
                            "Applying... {0}%", x);
                    });

                    logger.Info(
                        "A new update has been applied. Restarting...");
                    UpdateManager.RestartApp();
                }
                else
                {
                    logger.Info(
                        "No update available.");
                    return;
                }
            }
            catch (Exception)
            {
                logger.Error(
                    "Something went wrong when trying to update app.");
            }

            watchStart.Stop();
            var elapsedStartMs = watchStart.ElapsedMilliseconds;
            logger.Info(
                "Finished looking for updates.", elapsedStartMs);
        }
        #endregion

        #endregion
    }
}
