using System.Collections.Generic;
using Popcorn.Models.Movie.Json;

namespace Popcorn.Models.Movie
{
    /// <summary>
    /// Represents partial details of a movie
    /// </summary>
    public class MovieShort : MovieShortDeserialized
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

        #region Property -> CoverImagePath

        private string _coverImagePath = string.Empty;

        /// <summary>
        /// Local path of the downloaded movie's cover image
        /// </summary>
        public string CoverImagePath
        {
            get { return _coverImagePath; }
            set { Set(() => CoverImagePath, ref _coverImagePath, value); }
        }

        #endregion

        #endregion

        #region Property -> IsFavorite

        private bool _isFavorite;

        /// <summary>
        /// Indicate if movie is favorite
        /// </summary>
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { Set(() => IsFavorite, ref _isFavorite, value); }
        }

        #endregion

        #region Property -> HasBeenSeen

        private bool _hasBeenSeen;

        /// <summary>
        /// Indicate if movie has been seen by the user
        /// </summary>
        public bool HasBeenSeen
        {
            get { return _hasBeenSeen; }
            set { Set(() => HasBeenSeen, ref _hasBeenSeen, value); }
        }

        #endregion
    }
}