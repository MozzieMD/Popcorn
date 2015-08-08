using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;

namespace Popcorn.ViewModel.Players
{
    /// <summary>
    /// Manage media player
    /// </summary>
    public class MediaPlayerViewModel : ViewModelBase
    {
        #region Property -> MovieUri

        /// <summary>
        /// Uri to file path of the media to be played
        /// </summary>
        public Uri MediaUri { get; protected set; }

        #endregion

        #region Property -> IsInFullScreenMode

        private bool _isInFullScreenMode;

        /// <summary>
        /// Indicates if player is in fullscreen mode
        /// </summary>
        public bool IsInFullScreenMode
        {
            get { return _isInFullScreenMode; }
            set { Set(() => IsInFullScreenMode, ref _isInFullScreenMode, value); }
        }

        #endregion

        #region Commands

        #region Command -> ChangeScreenModeCommand

        private RelayCommand _changeScreenModeCommand;

        /// <summary>
        /// Command used to change screen mode (fullscreen or boxed)
        /// </summary>
        public RelayCommand ChangeScreenModeCommand
        {
            get
            {
                return _changeScreenModeCommand ??
                       (_changeScreenModeCommand =
                           new RelayCommand(
                               () => Messenger.Default.Send(new ChangeScreenModeMessage(IsInFullScreenMode))));
            }
        }

        #endregion

        #region Command -> StopPlayingMediaCommand

        /// <summary>
        /// Command used to stop playing the movie
        /// </summary>
        public RelayCommand StopPlayingMediaCommand { get; set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MediaPlayerViewModel class.
        /// </summary>
        protected MediaPlayerViewModel()
        {
            Messenger.Default.Register<ChangeScreenModeMessage>(
                this,
                message => { IsInFullScreenMode = message.IsFullScreen; });
        }

        #endregion

        #region Event -> OnStoppedPlayingMedia

        /// <summary>
        /// Event fired on stopped playing the media
        /// </summary>
        public event EventHandler<EventArgs> StoppedPlayingMedia;

        /// <summary>
        /// Fire StoppedPlayingMedia event
        /// </summary>
        ///<param name="e">Event data</param>
        protected void OnStoppedPlayingMedia(EventArgs e)
        {
            var handler = StoppedPlayingMedia;
            handler?.Invoke(this, e);
        }

        #endregion
    }
}