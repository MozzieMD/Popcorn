using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using Popcorn.Service.User;
using Popcorn.ViewModel.Tabs;

namespace Popcorn.ViewModel.Players
{
    public class MoviePlayerViewModel : ViewModelBase, ITab
    {

        #region Property -> ApiService

        /// <summary>
        /// The service used to consume APIs
        /// </summary>
        public IUserDataService UserDataService { get; }

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

        #region Property -> MediaUri

        /// <summary>
        /// Uri to file path of the media to be played
        /// </summary>
        public readonly Uri MediaUri;

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

        #region Property -> Movie

        /// <summary>
        /// Movie
        /// </summary>
        public readonly MovieFull Movie;

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
        /// Constructor
        /// </summary>
        /// <param name="movie">Movie to play</param>
        /// <param name="uri">Movie's media Uri</param>
        public MoviePlayerViewModel(MovieFull movie, Uri uri)
        {
            UserDataService = SimpleIoc.Default.GetInstance<IUserDataService>();

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
        /// Event fired on stopped playing the movie
        /// </summary>
        public event EventHandler<EventArgs> StoppedPlayingMovie;

        /// <summary>
        /// Fire StoppedPlayingMovie event
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnStoppedPlayingMovie(EventArgs e)
        {
            var handler = StoppedPlayingMovie;
            handler?.Invoke(this, e);
        }

        #endregion
    }
}
