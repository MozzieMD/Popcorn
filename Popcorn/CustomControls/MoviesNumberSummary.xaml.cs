using System.Windows;

namespace Popcorn.CustomControls
{
    /// <summary>
    /// Interaction logic for MoviesNumberSummary.xaml
    /// </summary>
    public partial class MoviesNumberSummary
    {
        #region DependencyProperties

        #region DependencyPropertiy -> MaxNumberOfMoviesProperty

        /// <summary>
        /// MaxNumberOfMoviesProperty
        /// </summary>
        public static readonly DependencyProperty MaxNumberOfMoviesProperty =
            DependencyProperty.Register("MaxNumberOfMovies",
                typeof (double), typeof (MoviesNumberSummary),
                new PropertyMetadata(0.0, OnMaximumNumberOfMoviesChanged));

        #endregion

        #region DependencyPropertiy -> MaxNumberOfMoviesProperty

        /// <summary>
        /// CurrentNumberOfMoviesProperty
        /// </summary>
        public static readonly DependencyProperty CurrentNumberOfMoviesProperty =
            DependencyProperty.Register("CurrentNumberOfMovies",
                typeof (double), typeof (MoviesNumberSummary),
                new PropertyMetadata(0.0, OnCurrentNumberOfMoviesChanged));

        #endregion

        #endregion

        #region Properties

        #region Property -> MaxNumberOfMovies

        /// <summary>
        /// The maximum number of movies
        /// </summary>
        public double MaxNumberOfMovies
        {
            get { return (double) GetValue(MaxNumberOfMoviesProperty); }
            set { SetValue(MaxNumberOfMoviesProperty, value); }
        }

        #endregion

        #region Property -> MaxNumberOfMovies

        /// <summary>
        /// The maximum number of movies
        /// </summary>
        public double CurrentNumberOfMovies
        {
            get { return (double) GetValue(CurrentNumberOfMoviesProperty); }
            set { SetValue(CurrentNumberOfMoviesProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of MoviesNumberSummary
        /// </summary>
        public MoviesNumberSummary()
        {
            InitializeComponent();
        }

        #endregion

        #region Method -> OnMaximumNumberOfMoviesChanged

        /// <summary>
        /// On number of movies changed
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Event args</param>
        private static void OnMaximumNumberOfMoviesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var moviesNumberSummary = d as MoviesNumberSummary;
            var newValue = e.NewValue as double?;
            if (newValue.HasValue)
                moviesNumberSummary?.FormatSummaryWithMaximumNumberOfMovies(newValue.Value);
        }

        #endregion

        #region Method -> OnCurrentNumberOfMoviesChanged

        /// <summary>
        /// On number of movies changed
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Event args</param>
        private static void OnCurrentNumberOfMoviesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var moviesNumberSummary = d as MoviesNumberSummary;
            var newValue = e.NewValue as double?;
            if (newValue.HasValue)
                moviesNumberSummary?.FormatSummaryWithMaxNumberOfMovies(newValue.Value);
        }

        #endregion

        #region Method -> FormatSummaryWithMaximumNumberOfMovies

        /// <summary>
        /// Format summary with the maximum movie's number
        /// </summary>
        /// <param name="maxNumberOfMovies">The maximum number of movies</param>
        private void FormatSummaryWithMaximumNumberOfMovies(double maxNumberOfMovies)
        {
            NumericTextBlock.Text = $"{CurrentNumberOfMovies} movies of {maxNumberOfMovies}";
        }

        #endregion

        #region Method -> FormatSummaryWithCurrentNumberOfMovies

        /// <summary>
        /// Format summary with the maximum movie's number
        /// </summary>
        /// <param name="currentNumberOfMovies">The current number of movies</param>
        private void FormatSummaryWithMaxNumberOfMovies(double currentNumberOfMovies)
        {
            NumericTextBlock.Text = $"{currentNumberOfMovies} movies of {MaxNumberOfMovies}";
        }

        #endregion
    }
}
