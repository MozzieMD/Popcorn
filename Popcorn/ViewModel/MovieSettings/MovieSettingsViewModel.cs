using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using Popcorn.Model.Subtitle;
using Popcorn.ViewModel.Subtitles;
using System.Threading.Tasks;

namespace Popcorn.ViewModel.MovieSettings
{
    public class MovieSettingsViewModel : ViewModelBase
    {
        #region Property -> Movie

        /// <summary>
        /// The movie
        /// </summary>
        private MovieFull _movie;

        public MovieFull Movie
        {
            get { return _movie; }
            set { Set(() => Movie, ref _movie, value); }
        }

        #endregion

        #region Property -> Subtitle

        /// <summary>
        /// The movie's subtitle
        /// </summary>
        private Subtitle Subtitle { get; set; }

        #endregion

        #region Property -> Subtitles

        /// <summary>
        /// The view model used to manage subtitles
        /// </summary>
        private SubtitlesViewModel _subtitles;

        public SubtitlesViewModel Subtitles
        {
            get { return _subtitles; }
            set { Set(() => Subtitles, ref _subtitles, value); }
        }

        #endregion

        #region Command -> SetSubtitlesCommand

        /// <summary>
        /// SetSubtitlesCommand
        /// </summary>
        private RelayCommand _setSubtitlesCommand;

        public RelayCommand SetSubtitlesCommand
        {
            get
            {
                return _setSubtitlesCommand ?? (_setSubtitlesCommand = new RelayCommand(async () =>
                {
                    await Task.Run(() =>
                    {
                        if (Subtitles == null)
                        {
                            Subtitles = new SubtitlesViewModel(Movie);
                        }
                        else
                        {
                            Subtitles = null;
                        }
                    });
                }));
            }
        }

        #endregion

        #region Command -> DownloadMovieCommand

        /// <summary>
        /// DownloadMovieCommand
        /// </summary>
        private RelayCommand _downloadMovieCommand;

        public RelayCommand DownloadMovieCommand
        {
            get
            {
                return _downloadMovieCommand ?? (_downloadMovieCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new DownloadMovieMessage(Movie, Subtitle));
                }));
            }
        }

        #endregion

        #region Command -> CancelCommand

        /// <summary>
        /// DownloadMovieCommand
        /// </summary>
        private RelayCommand _cancelCommand;

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
        /// Constructor
        /// </summary>
        /// <param name="movie">The movie</param>
        public MovieSettingsViewModel(MovieFull movie)
        {
            Movie = movie;

            Messenger.Default.Register<SelectedSubtitleMessage>(
            this,
            message =>
            {
                Subtitle = message.Subtitle;
            });
        }
        #endregion

        public override void Cleanup()
        {
            Messenger.Default.Unregister<SelectedSubtitleMessage>(this);

            base.Cleanup();
        }
    }
}
