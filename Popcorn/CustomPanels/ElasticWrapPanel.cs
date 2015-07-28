using System;
using System.Windows;
using System.Windows.Controls;
using Popcorn.Events;

namespace Popcorn.CustomPanels
{
    /// <summary>
    /// Custom panel which resize elements and stretch them in itself when window is resizing
    /// </summary>
    public class ElasticWrapPanel : Panel
    {
        #region DependencyProperties

        #region DependencyProperty -> DesiredColumnWidthProperty
        /// <summary>
        /// Identifies the <see cref="DesiredColumnWidth"/> dependency property. 
        /// </summary>
        internal static readonly DependencyProperty DesiredColumnWidthProperty = DependencyProperty.Register("DesiredColumnWidth", typeof(double), typeof(ElasticWrapPanel), new PropertyMetadata(230d, OnDesiredColumnWidthChanged));
        #endregion

        #endregion

        #region Properties

        #region Property -> Columns
        /// <summary>
        /// Columns
        /// </summary>
        private int _columns;
        public int Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                if (_columns != value)
                {
                    _columns = value;
                    OnNumberOfColumnsChanged(new NumberOfColumnChangedEventArgs(value));
                }
            }
        }
        #endregion

        #region Property -> DesiredColumnWidth
        /// <summary>
        /// DesiredColumnWidth 
        /// </summary>
        public double DesiredColumnWidth
        {
            private get
            {
                return (double)GetValue(DesiredColumnWidthProperty);
            }

            set
            {
                SetValue(DesiredColumnWidthProperty, value);
            }
        }
        #endregion

        #endregion

        #region Methods

        #region Method -> MeasureOverride
        /// <summary>
        /// Calculate the available space for each column
        /// </summary>
        /// <param name="availableSize">availableSize</param>
        /// <returns>Computed overrided size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Height.Equals(0))
            {
                availableSize.Height = MaxHeight;
            }

            Columns = (int)(availableSize.Width / DesiredColumnWidth);

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
            }

            return base.MeasureOverride(availableSize);
        }
        #endregion

        #region Method -> ArrangeOverride
        /// <summary>
        /// Calculate the size of each column to organize their location
        /// </summary>
        /// <param name="finalSize">finalSize</param>
        /// <returns>Computed arranged size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Columns != 0)
            {
                var columnWidth = Math.Floor(finalSize.Width / Columns);
                var totalHeight = 0.0;
                var top = 0.0;
                var rowHeight = 0.0;
                var overflow = 0.0;
                var column = 0;
                var index = 0;
                var overflowAlreadyCount = false;

                foreach (UIElement child in Children)
                {
                    // Compute the tile size and position
                    child.Arrange(new Rect(columnWidth * column, top, columnWidth, child.DesiredSize.Height));
                    column++;
                    rowHeight = Children.Count >= Columns ?
                         Math.Max(rowHeight, child.DesiredSize.Height) : Math.Min(rowHeight, child.DesiredSize.Height);
                    index++;

                    // Check if the current element is at the end of a row and add an height overflow to get enough space for the next elements of the second row
                    if (column == Columns && Children.Count != index && (Children.Count - index + 1) <= Columns && !overflowAlreadyCount)
                    {
                        overflow = rowHeight;
                        totalHeight += rowHeight;
                        overflowAlreadyCount = true;
                    }
                    else
                    {
                        if (!overflowAlreadyCount)
                        {
                            totalHeight += rowHeight;
                        }
                    }

                    if (column == Columns)
                    {
                        column = 0;
                        top += rowHeight;
                        rowHeight = 0;
                    }
                }

                if (Children.Count >= Columns)
                {
                    totalHeight = totalHeight/Columns + overflow;
                }

                Height = totalHeight;
                finalSize.Height = totalHeight;

            }
            return base.ArrangeOverride(finalSize);
        }
        #endregion

        #region Method -> OnDesiredColumnWidthChanged
        /// <summary>
        /// Inform when DesiredColumnWidthProperty has changed
        /// </summary>
        /// <param name="e">e</param>
        /// <param name="obj">obj</param>
        private static void OnDesiredColumnWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var panel = (ElasticWrapPanel)obj;
            panel.InvalidateMeasure();
            panel.InvalidateArrange();
        }
        #endregion

        #region Events

        #region Event -> NumberOfColumnsChanged
        /// <summary>
        /// NumberOfColumnsChanged event
        /// </summary>
        public event EventHandler<NumberOfColumnChangedEventArgs> NumberOfColumnsChanged;
        /// <summary>
        /// Inform when the current number of columns changed in the ElasticWrapPanel
        /// </summary>
        ///<param name="e">e</param>
        private void OnNumberOfColumnsChanged(NumberOfColumnChangedEventArgs e)
        {
            var handler = NumberOfColumnsChanged;
            handler?.Invoke(this, e);
        }
        #endregion

        #endregion

        #endregion
    }
}
