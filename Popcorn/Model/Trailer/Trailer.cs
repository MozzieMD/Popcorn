using GalaSoft.MvvmLight;
using System;

namespace Popcorn.Model.Trailer
{
    public class Trailer : ObservableObject
    {
        #region Property -> Uri

        private Uri _uri;

        /// <summary>
        /// Uri to the decrypted Youtube URL
        /// </summary>
        public Uri Uri
        {
            get { return _uri; }
            set { Set(() => Uri, ref _uri, value); }
        }

        #endregion

        #region
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uri"></param>
        public Trailer(Uri uri)
        {
            Uri = uri;
        }

        #endregion
    }
}
