using GalaSoft.MvvmLight;
using System;

namespace Popcorn.Models.Trailer
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
            private set { Set(() => Uri, ref _uri, value); }
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
