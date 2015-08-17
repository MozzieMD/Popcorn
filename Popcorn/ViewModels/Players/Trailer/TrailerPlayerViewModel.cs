using System;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;

namespace Popcorn.ViewModels.Players.Trailer
{
    /// <summary>
    /// Manage trailer player
    /// </summary>
    public sealed class TrailerPlayerViewModel : MediaPlayerViewModel
    {
        #region Property -> Trailer

        private Models.Trailer.Trailer _trailer;

        /// <summary>
        /// The trailer
        /// </summary>
        public Models.Trailer.Trailer Trailer
        {
            get { return _trailer; }
            private set { Set(() => Trailer, ref _trailer, value); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TrailerPlayerViewModel class.
        /// </summary>
        /// <param name="trailer">The trailer</param>
        public TrailerPlayerViewModel(Models.Trailer.Trailer trailer)
        {
            RegisterCommands();
            Trailer = trailer;
        }

        #endregion

        #region Methods

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            StopPlayingMediaCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new StopPlayingTrailerMessage());
            });
        }

        #endregion

        public override void Cleanup()
        {
            OnStoppedPlayingMedia(new EventArgs());

            base.Cleanup();
        }

        #endregion
    }
}