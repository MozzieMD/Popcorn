using System;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;

namespace Popcorn.ViewModel.Players.Trailer
{
    /// <summary>
    /// Manage trailer player
    /// </summary>
    public sealed class TrailerPlayerViewModel : MediaPlayerViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TrailerPlayerViewModel class.
        /// </summary>
        /// <param name="trailerUri">Trailer's media Uri</param>
        public TrailerPlayerViewModel(Uri trailerUri)
        {
            RegisterMessages();

            RegisterCommands();

            MediaUri = trailerUri;
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
                message => { OnStoppedPlayingMedia(new EventArgs()); });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
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
    }
}