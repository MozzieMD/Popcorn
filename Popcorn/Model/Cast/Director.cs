using Popcorn.Model.Cast.Json;

namespace Popcorn.Model.Cast
{
    public class Director : DirectorDeserialized
    {
        #region Properties

        #region Property -> SmallImagePath

        private string _smallImagePath = string.Empty;

        /// <summary>
        /// Local path of the downloaded director's small-sized image
        /// </summary>
        public string SmallImagePath
        {
            get { return _smallImagePath; }
            set { Set(() => SmallImagePath, ref _smallImagePath, value); }
        }

        #endregion

        #endregion
    }
}