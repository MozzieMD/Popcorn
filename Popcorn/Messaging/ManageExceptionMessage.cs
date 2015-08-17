using System;
using GalaSoft.MvvmLight.Messaging;

namespace Popcorn.Messaging
{
    /// <summary>
    /// Used to broadcast an exception
    /// </summary>
    public class ManageExceptionMessage : MessageBase
    {
        #region Properties

        #region Property -> Message

        /// <summary>
        /// The unhandled exception
        /// </summary>
        public readonly Exception UnHandledException;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unHandledException">The exception to broadcast</param>
        public ManageExceptionMessage(Exception unHandledException)
        {
            UnHandledException = unHandledException;
        }

        #endregion
    }
}