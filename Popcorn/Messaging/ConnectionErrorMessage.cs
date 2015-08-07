using GalaSoft.MvvmLight.Messaging;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast a connection error
    /// </summary>
    public class ConnectionErrorMessage : MessageBase
    {
        #region Properties

        #region Property -> Message

        /// <summary>
        /// Message to display
        /// </summary>
        public readonly string Message;

        #endregion

        #region Property -> ResetConnectionError

        /// <summary>
        /// Cancel the error (when retry e.g)
        /// </summary>
        public readonly bool ResetConnectionError;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        /// <param name="resetConnectionError">If we have to retrieve for a connection</param>
        public ConnectionErrorMessage(string message, bool resetConnectionError = false)
        {
            Message = message;
            ResetConnectionError = resetConnectionError;
        }

        #endregion
    }
}