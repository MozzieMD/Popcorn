using System;
using System.Windows;
using Popcorn.Helpers;

namespace Popcorn.Controls
{
    /// <summary>
    /// Interaction logic for DownloadMovieProgress.xaml
    /// </summary>
    public partial class DownloadMovieProgress
    {
        #region DependencyProperties

        #region DependencyPropertiy -> DownloadProgressProperty

        /// <summary>
        /// DownloadProgressProperty
        /// </summary>
        public static readonly DependencyProperty DownloadProgressProperty =
            DependencyProperty.Register("DownloadProgress",
                typeof (double), typeof (DownloadMovieProgress),
                new PropertyMetadata(0.0, OnDownloadProgressChanged));

        #endregion

        #region DependencyPropertiy -> DownloadRateProperty

        /// <summary>
        /// DownloadRateProperty
        /// </summary>
        public static readonly DependencyProperty DownloadRateProperty =
            DependencyProperty.Register("DownloadRate",
                typeof (double), typeof (DownloadMovieProgress),
                new PropertyMetadata(0.0));

        #endregion

        #region DependencyPropertiy -> MovieTitle

        /// <summary>
        /// MovieTitleProperty
        /// </summary>
        public static readonly DependencyProperty MovieTitleProperty =
            DependencyProperty.Register("MovieTitle",
                typeof (string), typeof (DownloadMovieProgress),
                new PropertyMetadata(string.Empty));

        #endregion

        #endregion

        #region Properties

        #region Property -> DownloadProgress

        /// <summary>
        /// The movie download progress
        /// </summary>
        public double DownloadProgress
        {
            get { return (double) GetValue(DownloadProgressProperty); }
            set { SetValue(DownloadProgressProperty, value); }
        }

        #endregion

        #region Property -> DownloadRate

        /// <summary>
        /// The movie download rate
        /// </summary>
        public double DownloadRate
        {
            get { return (double) GetValue(DownloadRateProperty); }
            set { SetValue(DownloadRateProperty, value); }
        }

        #endregion

        #region Property -> MovieTitle

        /// <summary>
        /// The movie title
        /// </summary>
        public string MovieTitle
        {
            get { return (string) GetValue(MovieTitleProperty); }
            set { SetValue(MovieTitleProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of DownloadMovieProgress
        /// </summary>
        public DownloadMovieProgress()
        {
            InitializeComponent();
        }

        #endregion

        #region Method -> OnDownloadProgressChanged

        /// <summary>
        /// On download progress changed
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Event args</param>
        private static void OnDownloadProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var downloadMovieProgress = d as DownloadMovieProgress;
            downloadMovieProgress?.DisplayDownloadProgress();
        }

        #endregion

        #region Method -> DisplayDownloadProgress

        /// <summary>
        /// Display download progress
        /// </summary>
        private void DisplayDownloadProgress()
        {
            if (DownloadProgress >= 2.0)
            {
                DisplayText.Text =
                    $"{LocalizationProviderHelper.GetLocalizedValue<string>("CurrentlyPlayingLabel")} : {MovieTitle}";
            }
            else
            {
                if (DownloadRate >= 1000)
                {
                    DisplayText.Text =
                        $"{LocalizationProviderHelper.GetLocalizedValue<string>("BufferingLabel")} : {Math.Round(DownloadProgress*50, 0)} % ({DownloadRate/1000} MB/s)";
                }
                else
                {
                    DisplayText.Text =
                        $"{LocalizationProviderHelper.GetLocalizedValue<string>("BufferingLabel")} : {Math.Round(DownloadProgress*50, 0)} % ({DownloadRate} kB/s)";
                }
            }
        }

        #endregion
    }
}
