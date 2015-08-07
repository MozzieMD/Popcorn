using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Popcorn.Converters
{
    /// <summary>
    /// Format string genres to add "/" character between each genre
    /// </summary>
    public class GenresConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Used to add "/" character at the end of each genre
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Formated string</returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var result = string.Empty;
            var index = 0;
            var genres = value as List<string>;
            if (genres != null)
            {
                foreach (var genre in genres)
                {
                    index++;

                    result += genre;
                    // Add the comma at the end of each genre.
                    if (index != genres.Count())
                    {
                        result += ", ";
                    }
                }
            }

            return result;
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