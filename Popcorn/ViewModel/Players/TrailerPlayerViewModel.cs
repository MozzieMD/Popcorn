using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;

namespace Popcorn.ViewModel.Players
{
    public class TrailerPlayerViewModel : ViewModelBase
    {
        #region Property -> MovieUri

        /// <summary>
        /// Uri to file path of the media to be played
        /// </summary>
        public readonly Uri TrailerUri;

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

        /// <summary>
        /// Command used to change screen mode (fullscreen or boxed)
        /// </summary>
        public RelayCommand ChangeScreenModeCommand { get; private set; }

        #endregion

        #region Command -> StopPlayingMediaCommand

        /// <summary>
        /// Command used to stop playing the movie
        /// </summary>
        public RelayCommand StopPlayingMediaCommand { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TrailerPlayerViewModel class.
        /// </summary>
        /// <param name="trailerUri">Trailer's media Uri</param>
        public TrailerPlayerViewModel(Uri trailerUri)
        {
            RegisterMessages();

            RegisterCommands();

            TrailerUri = trailerUri;
        }

        #endregion

        #region Methods

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<StopPlayingTrailerMessage>(
                this,
                message => { OnStoppedPlayingTrailer(new EventArgs()); });

            Messenger.Default.Register<ChangeScreenModeMessage>(
                this,
                message => { IsInFullScreenMode = message.IsFullScreen; });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            ChangeScreenModeCommand =
                new RelayCommand(() => { Messenger.Default.Send(new ChangeScreenModeMessage(IsInFullScreenMode)); });

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

        #endregion

        #region Event -> OnStoppedPlayingTrailer

        /// <summary>
        /// Event fired on stopped playing the trailer
        /// </summary>
        public event EventHandler<EventArgs> StoppedPlayingTrailer;

        /// <summary>
        /// Fire StoppedPlayingTrailer event
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnStoppedPlayingTrailer(EventArgs e)
        {
            var handler = StoppedPlayingTrailer;
            handler?.Invoke(this, e);
        }

        #endregion
    }
}