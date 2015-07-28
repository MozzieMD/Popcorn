using System;

namespace Popcorn.Events
{
    /// <summary>
    /// Used to transmit the updated number of columns of the elastic panel
    /// </summary>
    public class NumberOfColumnChangedEventArgs : EventArgs
    {
        #region Properties

        #region Property -> NumberOfColumns
        /// <summary>
        /// Number of columns
        /// </summary>
        public int NumberOfColumns { get; private set; }
        #endregion

        #endregion
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numberOfColumns">Number of columns</param>
        public NumberOfColumnChangedEventArgs(int numberOfColumns)
        {
            NumberOfColumns = numberOfColumns;
        }
        #endregion
    }
}
