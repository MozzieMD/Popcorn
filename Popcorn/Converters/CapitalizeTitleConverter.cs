using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

namespace Popcorn.Converters
{
    /// <summary>
    /// Convert to a capitalized string
    /// </summary>
    public class CapitalizeTitleConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Convert to a capitalized string
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Capitalized string</returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var title = value as string;
            if (title == null)
                return default(string);
            
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(title);
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