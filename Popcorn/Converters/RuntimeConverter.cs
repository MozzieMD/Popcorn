using System;
using System.Globalization;
using System.Windows.Data;

namespace Popcorn.Converters
{
    /// <summary>
    /// Used to convert raw runtime movie to formated runtime
    /// </summary>
    public class RuntimeConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Convert to readable runtime
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Formated runtime (..h..m)</returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var runtime = string.Empty;
            var result = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);

            if (result >= 60.0)
            {
                var hours = result/60.0;
                var minutes = result%60.0;

                if (minutes < 10.0)
                {
                    runtime = Math.Floor(hours) + "h" + "0" + minutes;
                }
                else
                {
                    runtime = Math.Floor(hours) + "h" + minutes;
                }
            }

            return runtime;
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}