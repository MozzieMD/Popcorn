using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using Popcorn.ViewModel.Subtitles;

namespace Popcorn.ViewModel.MovieSettings
{
    /// <summary>
    /// Manage the movie's playing settings
    /// </summary>
    public sealed class MovieSettingsViewModel : ViewModelBase
    {
        #region Property -> Movie

        private MovieFull _movie;

        /// <summary>
        /// The movie
        /// </summary>
        public MovieFull Movie
        {
            get { return _movie; }
            set { Set(() => Movie, ref _movie, value); }
        }

        #endregion

        #region Property -> Subtitles

        private SubtitlesViewModel _subtitles;

        /// <summary>
        /// The view model used to manage subtitles
        /// </summary>
        public SubtitlesViewModel Subtitles
        {
            get { return _subtitles; }
            set { Set(() => Subtitles, ref _subtitles, value); }
        }

        #endregion

        #region Command -> SetSubtitlesCommand


        private RelayCommand _setSubtitlesCommand;

        /// <summary>
        /// Used to enable or disable subtitles
        /// </summary>
        public RelayCommand SetSubtitlesCommand
        {
            get
            {
                return _setSubtitlesCommand ?? (_setSubtitlesCommand = new RelayCommand(async () =>
                {
                    Movie.SelectedSubtitle = null;
                    if (Subtitles == null)
                    {
                        Subtitles = await SubtitlesViewModel.CreateAsync(Movie);
                    }
                    else
                    {
                        Subtitles.Cleanup();
                        Subtitles = null;
                    }
                }));
            }
        }

        #endregion

        #region Command -> DownloadMovieCommand

        private RelayCommand _downloadMovieCommand;

        /// <summary>
        /// Command used to download the movie
        /// </summary>
        public RelayCommand DownloadMovieCommand
        {
            get
            {
                return _downloadMovieCommand ?? (_downloadMovieCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new DownloadMovieMessage(Movie));
                }));
            }
        }

        #endregion

        #region Command -> CancelCommand

        private RelayCommand _cancelCommand;

        /// <summary>
        /// Command used to cancel the download of a movie
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new StopPlayingMovieMessage());
                }));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MovieSettingsViewModel class.
        /// </summary>
        /// <param name="movie">The movie</param>
        public MovieSettingsViewModel(MovieFull movie)
        {
            Movie = movie;
        }

        #endregion

        #region Methods

        public override void Cleanup()
        {
            Subtitles?.Cleanup();
            base.Cleanup();
        }

        #endregion
    }
}