using System;
using System.Globalization;
using System.Windows;

namespace Popcorn.Controls
{
    /// <summary>
    /// Interaction logic for MovieRuntime.xaml
    /// </summary>
    public partial class MovieRuntime
    {
        #region DependencyProperties

        #region DependencyPropertiy -> RuntimeProperty

        /// <summary>
        /// RuntimeProperty
        /// </summary>
        public static readonly DependencyProperty RuntimeProperty =
            DependencyProperty.Register("Runtime",
                typeof(double), typeof(MovieRuntime),
                new PropertyMetadata(0.0, OnRuntimeChanged));

        #endregion

        #endregion

        #region Properties

        #region Property -> Runtime

        /// <summary>
        /// The movie runtime
        /// </summary>
        public double Runtime
        {
            get { return (double)GetValue(RuntimeProperty); }
            set { SetValue(RuntimeProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of MovieRuntime
        /// </summary>
        public MovieRuntime()
        {
            InitializeComponent();
        }

        #endregion

        #region Method -> OnRuntimeChanged

        /// <summary>
        /// On movie runtime changed
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Event args</param>
        private static void OnRuntimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var movieRuntime = d as MovieRuntime;
            movieRuntime?.DisplayMovieRuntime();
        }

        #endregion

        #region Method -> DisplayMovieRuntime

        /// <summary>
        /// Display movie runtime
        /// </summary>
        private void DisplayMovieRuntime()
        {
            var result = Convert.ToDouble(Runtime, CultureInfo.InvariantCulture);
            if (result >= 60.0)
            {
                var hours = result / 60.0;
                var minutes = result % 60.0;

                if (minutes < 10.0)
                {
                    DisplayText.Text = $"{Math.Floor(hours)}h0{minutes}";
                }
                else
                {
                    DisplayText.Text = $"{Math.Floor(hours)}h{minutes}";
                }
            }
        }

        #endregion
    }
}
