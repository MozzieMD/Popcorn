using System;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;
using Popcorn.Models.Movie;
using Popcorn.Services.User;
using Popcorn.ViewModels.Tabs;

namespace Popcorn.ViewModels.Players.Movie
{
    /// <summary>
    /// Manage movie player
    /// </summary>
    public sealed class MoviePlayerViewModel : MediaPlayerViewModel, ITab
    {
        #region Property -> MovieService

        /// <summary>
        /// The service used to consume APIs
        /// </summary>
        public UserService UserService { get; }

        #endregion

        #region Property -> TabName


        private string _tabName;

        /// <summary>
        /// The name of the tab shown into the interface
        /// </summary>
        public string TabName
        {
            get { return _tabName; }
            set { Set(() => TabName, ref _tabName, value); }
        }

        #endregion

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
            UserService = SimpleIoc.Default.GetInstance<UserService>();
            Movie = movie;
            TabName = !string.IsNullOrEmpty(Movie.Title) ? Movie.Title : Properties.Resources.PlayingTitleTab;
        }

        #endregion

        #region Methods

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