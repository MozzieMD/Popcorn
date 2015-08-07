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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly UpdateManager UpdateManager;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        static App()
        {
            var watchStart = Stopwatch.StartNew();

            Logger.Info(
                "Popcorn starting...");

            AppDomain.CurrentDomain.ProcessExit += (sender, args) => UpdateManager.Dispose();

            Directory.CreateDirectory(Helpers.Constants.Logging);

            DispatcherHelper.Initialize();

            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;

            UpdateManager = new UpdateManager(Helpers.Constants.UpdateServerUrl, Helpers.Constants.ApplicationName);

            watchStart.Stop();
            var elapsedStartMs = watchStart.ElapsedMilliseconds;
            Logger.Info(
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

            Logger.Info(
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
                    Logger.Error(
                        "Problem while trying to check new updates.");
                    return;
                }

                if (updateInfo.ReleasesToApply.Any())
                {
                    Logger.Info(
                        "A new update has been found!\nCurrently installed version: {0}\nNew update: {1}",
                        updateInfo.CurrentlyInstalledVersion?.Version?.Build,
                        updateInfo.FutureReleaseEntry?.Version?.Build);

                    await UpdateManager.DownloadReleases(updateInfo.ReleasesToApply, x =>
                    {
                        Logger.Info(
                            "Downloading new update... {0}%", x);
                    });

                    await UpdateManager.ApplyReleases(updateInfo, x =>
                    {
                        Logger.Info(
                            "Applying... {0}%", x);
                    });

                    Logger.Info(
                        "A new update has been applied. Restarting...");
                    UpdateManager.RestartApp();
                }
                else
                {
                    Logger.Info(
                        "No update available.");
                    return;
                }
            }
            catch (Exception)
            {
                Logger.Error(
                    "Something went wrong when trying to update app.");
            }

            watchStart.Stop();
            var elapsedStartMs = watchStart.ElapsedMilliseconds;
            Logger.Info(
                "Finished looking for updates.", elapsedStartMs);
        }

        #endregion

        #endregion
    }
}