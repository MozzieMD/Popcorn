using System;
using System.Net;

namespace Popcorn.Helpers
{
    /// <summary>
    /// WebClient with NoKeepAlive option
    /// </summary>
    public class NoKeepAliveWebClient : WebClient
    {
        #region Methods

        #region Method -> GetWebRequest
        /// <summary>
        /// Get a web request without the KeepAlive option
        /// </summary>
        /// <param name="address">Address to request</param>
        /// <returns>Web request</returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);

            var req = request as HttpWebRequest;
            if (req != null)
            {
                req.KeepAlive = false;
            }

            return request;
        }
        #endregion

        #endregion
    }
}
