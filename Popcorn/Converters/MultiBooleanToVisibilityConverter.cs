using System;
using System.Windows.Data;

namespace Popcorn.Converters
{
    /// <summary>
    /// Convert booleans to a visibility property
    /// </summary>
    public class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert booleans to a visibility property
        /// </summary>
        /// <param name="values">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Visible if all booleans are true, collapsed otherwise</returns>
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var visible = true;
            foreach (var value in values)
            {
                if (value is bool)
                {
                    visible = visible && (bool) value;
                }
            }

            return visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetTypes">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value,
            Type[] targetTypes,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}