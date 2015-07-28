using System;

namespace Popcorn.Events
{
    /// <summary>
    /// Used to transmit the error message on a connection error
    /// </summary>
    public class ConnectionErrorEventArgs : EventArgs
    {
        #region Properties

        #region Property -> Message
        /// <summary>
        /// Connection error message
        /// </summary>
        public string Message { get; private set; }
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Connection error message</param>
        public ConnectionErrorEventArgs(string message)
        {
            Message = message;
        }
        #endregion
    }
}
