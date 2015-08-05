using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Messaging;
using Popcorn.Model.Movie;
using Popcorn.ViewModel.Subtitles;

namespace Popcorn.ViewModel.MovieSettings
{
    public class MovieSettingsViewModel : ViewModelBase
    {
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
        public RelayCommand SetSubtitlesCommand { get; private set; }
    
        #endregion

        #region Command -> DownloadMovieCommand

        /// <summary>
        /// DownloadMovieCommand
        /// </summary>
        public RelayCommand DownloadMovieCommand { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="movie">The movie</param>
        public MovieSettingsViewModel(MovieFull movie)
        {
            SetSubtitlesCommand = new RelayCommand(() =>
            {
                if (Subtitles == null)
                {
                    Subtitles = new SubtitlesViewModel(movie);
                }
                else
                {
                    Subtitles = null;
                }
            });

            DownloadMovieCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new DownloadMovieMessage(movie));
            });
        }
        #endregion
    }
}
