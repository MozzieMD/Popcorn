using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Popcorn.Converters
{
    /// <summary>
    /// Used to convert a window state to a boolean
    /// </summary>
    [ValueConversion(typeof (WindowState), typeof (bool))]
    public class WindowStateToBooleanConverter : MarkupExtension, IValueConverter
    {
        private WindowStateToBooleanConverter _instance;

        #region IValueConverter Members

        /// <summary>
        /// Convert boolean to a window state
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>True if maximized, false otherwise</returns>
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var isFullscreen = (bool) value;
            if (isFullscreen)
            {
                return WindowState.Maximized;
            }

            return WindowState.Normal;
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var windowState = (WindowState) value;
            if (windowState == WindowState.Minimized || windowState == WindowState.Normal)
            {
                return false;
            }

            return true;
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new WindowStateToBooleanConverter());
        }
    }
}