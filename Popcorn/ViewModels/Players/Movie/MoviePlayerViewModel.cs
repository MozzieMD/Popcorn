using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;
using Popcorn.Models.Movie;

namespace Popcorn.ViewModels.Players.Movie
{
    /// <summary>
    /// Manage movie player
    /// </summary>
    public sealed class MoviePlayerViewModel : MediaPlayerViewModel
    {
        #region Property -> Movie

        /// <summary>
        /// Movie
        /// </summary>
        public readonly MovieFull Movie;

        #endregion

        #region Commands

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MoviePlayerViewModel class.
        /// </summary>
        /// <param name="movie">Movie to play</param>
        public MoviePlayerViewModel(MovieFull movie)
        {
            RegisterMessages();
            RegisterCommands();
            Movie = movie;
            TabName = !string.IsNullOrEmpty(Movie.Title) ? Movie.Title : Properties.Resources.PlayingTitleTab;
        }

        #endregion

        #region Methods

        #region -> Method
        /// <summary>
        /// When a movie has been seen, save this information in user data
        /// </summary>
        public async Task HasSeenMovie()
        {
            await UserService.SetHasBeenSeenMovieAsync(Movie);
            Messenger.Default.Send(new ChangeHasBeenSeenMovieMessage(Movie));
            Messenger.Default.Send(new StopPlayingMovieMessage());
        }
        #endregion

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<ChangeLanguageMessage>(
                this,
                language => { TabName = Movie.Title; });
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
                Messenger.Default.Send(new StopPlayingMovieMessage());
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