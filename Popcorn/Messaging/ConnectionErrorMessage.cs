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
        public string Message { get; private set; }
        #endregion

        #region Property -> ResetConnectionError
        /// <summary>
        /// Cancel the error (when retry e.g)
        /// </summary>
        public bool ResetConnectionError { get; private set; }
        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionErrorMessage(string message, bool resetConnectionError = false)
        {
            Message = message;
            ResetConnectionError = resetConnectionError;
        }
        #endregion
    }
}
