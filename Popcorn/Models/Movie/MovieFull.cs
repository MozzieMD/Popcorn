using System.Collections.Generic;
using Popcorn.Models.Movie.Json;
using System.Collections.ObjectModel;
using System;

namespace Popcorn.Models.Movie
{
    /// <summary>
    /// Represents all of the movie's details
    /// </summary>
    public class MovieFull : MovieFullDeserialized
    {
        #region Properties

        #region Property -> Title

        /// <summary>
        /// Movie's title
        /// </summary>
        public new string Title
        {
            get { return base.Title; }
            set { Set(() => Title, ref base.Title, value); }
        }

        #endregion

        #region Property -> DescriptionFull

        /// <summary>
        /// Movie's full description
        /// </summary>
        public new string DescriptionFull
        {
            get { return base.DescriptionFull; }
            set { Set(() => DescriptionFull, ref base.DescriptionFull, value); }
        }

        #endregion

        #region Property -> Genres

        /// <summary>
        /// Movie's genres
        /// </summary>
        public new IEnumerable<string> Genres
        {
            get { return base.Genres; }
            set { Set(() => Genres, ref base.Genres, value); }
        }

        #endregion

        #region Property -> FilePath

        private Uri _filePath;

        /// <summary>
        /// Local path of the downloaded movie file
        /// </summary>
        public Uri FilePath
        {
            get { return _filePath; }
            set { Set(() => FilePath, ref _filePath, value); }
        }

        #endregion

        #region Property -> BackgroundImagePath

        private string _backgroundImagePath = string.Empty;

        /// <summary>
        /// Local path of the downloaded movie's background image
        /// </summary>
        public string BackgroundImagePath
        {
            get { return _backgroundImagePath; }
            set { Set(() => BackgroundImagePath, ref _backgroundImagePath, value); }
        }

        #endregion

        #region Property -> PosterImagePath

        private string _posterImagePath = string.Empty;

        /// <summary>
        /// Local path of the downloaded movie's poster image
        /// </summary>
        public string PosterImagePath
        {
            get { return _posterImagePath; }
            set { Set(() => PosterImagePath, ref _posterImagePath, value); }
        }

        #endregion

        #region Property -> WatchInFullHdQuality


        private bool _watchInFullHdQuality;

        /// <summary>
        /// Decide if movie has to be watched in full HQ or not
        /// </summary>
        public bool WatchInFullHdQuality
        {
            get { return _watchInFullHdQuality; }
            set { Set(() => WatchInFullHdQuality, ref _watchInFullHdQuality, value); }
        }

        #endregion

        #region Property -> FullHdAvailable

        private bool _fullHdAvailable;

        /// <summary>
        /// Indicate if full HQ quality is available
        /// </summary>
        public bool FullHdAvailable
        {
            get { return _fullHdAvailable; }
            set { Set(() => FullHdAvailable, ref _fullHdAvailable, value); }
        }

        #endregion

        #region Property -> AvailableSubtitles

        private ObservableCollection<Subtitle.Subtitle> _availableSubtitles =
            new ObservableCollection<Subtitle.Subtitle>();

        /// <summary>
        /// Available subtitles
        /// </summary>
        public ObservableCollection<Subtitle.Subtitle> AvailableSubtitles
        {
            get { return _availableSubtitles; }
            set { Set(() => AvailableSubtitles, ref _availableSubtitles, value); }
        }

        #endregion

        #region Property -> SelectedSubtitle

        private Subtitle.Subtitle _selectedSubtitle;

        /// <summary>
        /// Selected subtitle
        /// </summary>
        public Subtitle.Subtitle SelectedSubtitle
        {
            get { return _selectedSubtitle; }
            set { Set(() => SelectedSubtitle, ref _selectedSubtitle, value); }
        }

        #endregion

        #endregion
    }
}