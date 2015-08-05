using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

namespace Popcorn.Converters
{
    /// <summary>
    /// Convert from rating string ("0" to "10") to an double (0.0 to 5.0)
    /// </summary>
    public class CapitalizeTitleConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Convert rating string ("0" to "10") to a double (0.0 to 5.0)
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Formated rating double</returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var title = value as string;
            if (title == null)
            {
                return default(string);
            }

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(title);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
