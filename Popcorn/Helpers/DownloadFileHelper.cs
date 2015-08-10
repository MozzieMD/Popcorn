using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Popcorn.Helpers
{
    public static class DownloadFileHelper
    {
        #region Logger

        /// <summary>
        /// Logger of the class
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Downloads a file from a specified Internet address.
        /// </summary>
        /// <param name="remotePath">Internet address of the file to download.</param>
        /// <param name="localPath">Local file name where to store the content of the download, if null a temporary file name will be generated.</param>
        /// <param name="timeOut">Duration in miliseconds before cancelling the  operation.</param>
        /// <param name="progress">Report the progress of the download</param>
        /// <param name="ct">Cancellation token</param>
        public static async Task<Tuple<string, string, Exception>> DownloadFileTaskAsync(string remotePath,
            string localPath = null, int timeOut = 10000, IProgress<long> progress = null, CancellationTokenSource ct = null)
        {
            try
            {
                var watch = Stopwatch.StartNew();

                if (remotePath == null)
                {
                    Logger.Debug("DownloadFileTaskAsync (null remote path): skipping");
                    throw new ArgumentNullException(nameof(remotePath));
                }

                if (localPath == null)
                {
                    Logger.Debug(
                        $"DownloadFileTaskAsync (null local path): generating a temporary file name for {remotePath}");
                    localPath = Path.GetTempFileName();
                }

                if (File.Exists(localPath))
                {
                    var fileInfo = new FileInfo(localPath).Length;
                    if (fileInfo != 0)
                    {
                        return new Tuple<string, string, Exception>(remotePath, localPath, null);
                    }
                }

                var direcory = Path.GetDirectoryName(localPath);
                if (!string.IsNullOrEmpty(direcory) && !Directory.Exists(direcory))
                {
                    Directory.CreateDirectory(direcory);
                }

                using (var client = new NoKeepAliveWebClient())
                {
                    if (progress != null)
                    {
                        client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
                        {
                            progress.Report(e.BytesReceived/e.TotalBytesToReceive);
                        };
                    }

                    TimerCallback timerCallback = c =>
                    {
                        var webClient = (WebClient) c;
                        if (!webClient.IsBusy) return;
                        webClient.CancelAsync();
                        Logger.Debug($"DownloadFileTaskAsync (time out due): {remotePath}");
                    };

                    using(ct?.Token.Register(() => client.CancelAsync()))
                    using (new Timer(timerCallback, client, timeOut, Timeout.Infinite))
                    {
                        await client.DownloadFileTaskAsync(remotePath, localPath);
                    }

                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;

                    Logger.Debug($"DownloadFileTaskAsync (downloaded in {elapsedMs} ms): {remotePath}");
                    return new Tuple<string, string, Exception>(remotePath, localPath, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"DownloadFileTaskAsync (download failed): {remotePath} Additional informations : {ex.Message}");
                return new Tuple<string, string, Exception>(remotePath, null, ex);
            }
        }
    }
}