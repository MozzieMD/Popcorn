using System.Windows;
using Popcorn.Helpers;

namespace Popcorn.Controls
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
                new PropertyMetadata(0.0, OnNumberOfMoviesChanged));

        #endregion

        #region DependencyPropertiy -> MaxNumberOfMoviesProperty

        /// <summary>
        /// CurrentNumberOfMoviesProperty
        /// </summary>
        public static readonly DependencyProperty CurrentNumberOfMoviesProperty =
            DependencyProperty.Register("CurrentNumberOfMovies",
                typeof (double), typeof (MoviesNumberSummary),
                new PropertyMetadata(0.0, OnNumberOfMoviesChanged));

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

        #region Method -> OnNumberOfMoviesChanged

        /// <summary>
        /// On number of movies changed
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Event args</param>
        private static void OnNumberOfMoviesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var moviesNumberSummary = d as MoviesNumberSummary;
            moviesNumberSummary?.DisplayMoviesNumberSummary();
        }

        #endregion

        #region Method -> DisplayMoviesNumberSummary

        /// <summary>
        /// Display movies summary
        /// </summary>
        private void DisplayMoviesNumberSummary()
        {
            if (CurrentNumberOfMovies.Equals(MaxNumberOfMovies))
            {
                MaxMovies.Visibility = Visibility.Collapsed;
                CurrentMovies.Visibility = Visibility.Visible;

                CurrentMovies.Text =
                    $"{CurrentNumberOfMovies}";
            }
            else
            {
                MaxMovies.Visibility = Visibility.Visible;
                CurrentMovies.Visibility = Visibility.Visible;

                CurrentMovies.Text =
                    $"{CurrentNumberOfMovies}";
                MaxMovies.Text =
                    $"{MaxNumberOfMovies}";
            }
        }

        #endregion
    }
}