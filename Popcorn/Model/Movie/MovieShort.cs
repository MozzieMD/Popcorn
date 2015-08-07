using System.Collections.Generic;
using Popcorn.Model.Movie.Json;

namespace Popcorn.Model.Movie
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

        #region Property -> IsLiked

        private bool? _isLiked;

        /// <summary>
        /// Indicate if movie has been liked by the user
        /// </summary>
        public bool? IsLiked
        {
            get { return _isLiked; }
            set { Set(() => IsLiked, ref _isLiked, value); }
        }

        #endregion

        #region Property -> IsLiked

        private bool? _isSeen;

        /// <summary>
        /// Indicate if movie has been seen by the user
        /// </summary>
        public bool? IsSeen
        {
            get { return _isSeen; }
            set { Set(() => IsSeen, ref _isSeen, value); }
        }

        #endregion
    }
}