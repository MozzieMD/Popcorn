using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace Popcorn.ViewModel.Players
{
    public class TrailerPlayerViewModel : ViewModelBase
    {
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
        /// TrailerPlayerViewModel
        /// </summary>
        /// <param name="uri">Trailer's media Uri</param>
        public TrailerPlayerViewModel(Uri uri)
        {
            MediaUri = uri;

            Messenger.Default.Register<StopPlayingTrailerMessage>(
            this,
            _ =>
            {
                OnStoppedPlayingTrailer(new EventArgs());
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

                Messenger.Default.Send(new StopPlayingTrailerMessage());
            });
        }

        #endregion

        #region Event -> OnStoppedPlayingTrailer

        /// <summary>
        /// StoppedPlayingTrailer event
        /// </summary>
        public event EventHandler<EventArgs> StoppedPlayingTrailer;

        /// <summary>
        /// Fire event when stop playing trailer
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnStoppedPlayingTrailer(EventArgs e)
        {
            var handler = StoppedPlayingTrailer;
            handler?.Invoke(this, e);
        }

        #endregion

        public override void Cleanup()
        {
            Messenger.Default.Unregister<StopPlayingTrailerMessage>(this);
            Messenger.Default.Unregister<ChangeScreenModeMessage>(this);

            base.Cleanup();
        }
    }
}
