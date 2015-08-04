using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using Popcorn.ViewModel.Tabs;
using GalaSoft.MvvmLight.Threading;

namespace Popcorn.ViewModel.Players
{
    public class MoviePlayerViewModel : ViewModelBase, ITab
    {
        #region Property -> TabName

        /// <summary>
        /// The name of the tab shown in the interface
        /// </summary>
        private string _tabName;

        public string TabName
        {
            get
            {
                return _tabName;
            }
            set
            {
                Set(() => TabName, ref _tabName, value);
            }
        }

        #endregion

        #region Property -> MediaUri

        /// <summary>
        /// Uri to file path of the media to be played
        /// </summary>
        public readonly Uri MediaUri;

        #endregion

        #region Property -> IsInFullScreenMode

        /// <summary>
        /// Indicates if player is in fullscreen mode
        /// </summary>
        private bool _isInFullScreenMode;

        public bool IsInFullScreenMode
        {
            get { return _isInFullScreenMode; }
            set { Set(() => IsInFullScreenMode, ref _isInFullScreenMode, value); }
        }

        #endregion

        #region Property -> Movie

        /// <summary>
        /// Movie
        /// </summary>
        public readonly MovieFull Movie;

        #endregion

        #region Commands

        #region Command -> ChangeScreenModeCommand

        /// <summary>
        /// ChangeScreenModeCommand
        /// </summary>
        public RelayCommand ChangeScreenModeCommand { get; private set; }

        #endregion

        #region Command -> StopPlayingMediaCommand

        /// <summary>
        /// StopPlayingMediaCommand
        /// </summary>
        public RelayCommand StopPlayingMediaCommand { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// MoviePlayerViewModel
        /// </summary>
        /// <param name="movie">Movie to play</param>
        /// <param name="uri">Movie's media Uri</param>
        public MoviePlayerViewModel(MovieFull movie, Uri uri)
        {
            MediaUri = uri;
            Movie = movie;

            TabName = !string.IsNullOrEmpty(Movie.Title) ? Movie.Title : Properties.Resources.PlayingTitleTab;

            Messenger.Default.Register<ChangeLanguageMessage>(
            this,
            language =>
            {
                TabName = Movie.Title;
            });

            Messenger.Default.Register<StopPlayingMovieMessage>(
            this,
            message =>
            {
                OnStoppedPlayingMovie(new EventArgs());
            });

            Messenger.Default.Register<ChangeScreenModeMessage>(
            this,
            message =>
            {
                IsInFullScreenMode = message.IsFullScreen;
            });

            ChangeScreenModeCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new ChangeScreenModeMessage(IsInFullScreenMode));
            });

            StopPlayingMediaCommand = new RelayCommand(() =>
            {
                if (IsInFullScreenMode)
                {
                    IsInFullScreenMode = !IsInFullScreenMode;
                    Messenger.Default.Send(new ChangeScreenModeMessage(IsInFullScreenMode));
                }

                Messenger.Default.Send(new StopPlayingMovieMessage());
            });
        }

        #endregion

        #region Event -> OnStoppedPlayingMovie

        /// <summary>
        /// StoppedDownloadingMovie event
        /// </summary>
        public event EventHandler<EventArgs> StoppedPlayingMovie;

        /// <summary>
        /// Fire event when movie has stopped downloading
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnStoppedPlayingMovie(EventArgs e)
        {
            var handler = StoppedPlayingMovie;
            handler?.Invoke(this, e);
        }

        #endregion

        public override void Cleanup()
        {
            Messenger.Default.Unregister<ChangeLanguageMessage>(this);
            Messenger.Default.Unregister<StopPlayingMovieMessage>(this);
            Messenger.Default.Unregister<ChangeScreenModeMessage>(this);

            base.Cleanup();
        }
    }
}
