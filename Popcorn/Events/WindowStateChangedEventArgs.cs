using System;

namespace Popcorn.Events
{
    /// <summary>
    /// Used to transmit a window state change
    /// </summary>
    public class WindowStateChangedEventArgs : EventArgs
    {
        #region Properties

        #region Property -> IsMoviePlaying

        /// <summary>
        /// Is in movie mode
        /// </summary>
        public readonly bool IsMoviePlaying;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isMoviePlaying">Is movie playing</param>
        public WindowStateChangedEventArgs(bool isMoviePlaying)
        {
            IsMoviePlaying = isMoviePlaying;
        }

        #endregion
    }
}