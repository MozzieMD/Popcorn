using System;
using System.Windows.Data;
using Popcorn.Helpers;

namespace Popcorn.Converters
{
    /// <summary>
    /// Report the download progress of a movie
    /// </summary>
    public class MovieDownloadProgressConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert booleans to an OR boolean
        /// </summary>
        /// <param name="values">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Visible if all booleans are true, collapsed otherwise</returns>
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var downloadProgress = values?.GetValue(0) as double?;
            var downloadRate = values?.GetValue(1) as double?;
            var movieTitle = values?.GetValue(2) as string;

            string result = string.Empty;
            if (downloadProgress != null && downloadRate != null && !string.IsNullOrEmpty(movieTitle))
            {
                if (downloadProgress >= 2.0)
                {
                    result = LocalizationProviderHelper.GetLocalizedValue<string>("CurrentlyPlayingLabel") +
                             " : " +
                             movieTitle;
                }
                else
                {
                    if (downloadRate >= 1000)
                    {
                        result = LocalizationProviderHelper.GetLocalizedValue<string>("BufferingLabel") +
                                 " : " +
                                 Math.Round(downloadProgress.Value*50, 0) +
                                 " %" +
                                 " ( " +
                                 downloadRate.Value/1000 +
                                 " MB/s)";
                    }
                    else
                    {
                        result = LocalizationProviderHelper.GetLocalizedValue<string>("BufferingLabel") +
                                 " : " +
                                 Math.Round(downloadProgress.Value*50, 0) +
                                 " %" +
                                 " ( " +
                                 downloadRate.Value +
                                 " kB/s)";
                    }
                }
            }

            return result;
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