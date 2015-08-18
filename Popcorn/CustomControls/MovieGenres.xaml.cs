using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Popcorn.CustomControls
{
    /// <summary>
    /// Interaction logic for MovieGenres.xaml
    /// </summary>
    public partial class MovieGenres
    {
        #region DependencyProperties

        #region DependencyPropertiy -> GenresProperty

        /// <summary>
        /// RuntimeProperty
        /// </summary>
        public static readonly DependencyProperty GenresProperty =
            DependencyProperty.Register("Genres",
                typeof(IEnumerable<string>), typeof(MovieGenres),
                new PropertyMetadata(null, OnGenresChanged));

        #endregion

        #endregion

        #region Properties

        #region Property -> Genres

        /// <summary>
        /// The movie runtime
        /// </summary>
        public IEnumerable<string> Genres
        {
            get { return (IEnumerable<string>)GetValue(GenresProperty); }
            set { SetValue(GenresProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of MovieGenres
        /// </summary>
        public MovieGenres()
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
        private static void OnGenresChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var movieGenres = d as MovieGenres;
            movieGenres?.DisplayMovieGenres();
        }

        #endregion

        #region Method -> DisplayMovieGenres

        /// <summary>
        /// Display movie genres
        /// </summary>
        private void DisplayMovieGenres()
        {
            var index = 0;
            if (Genres == null)
                return;

            foreach (var genre in Genres)
            {
                index++;
                NumericTextBlock.Text += genre;
                // Add the comma at the end of each genre.
                if (index != Genres.Count())
                {
                    NumericTextBlock.Text += ", ";
                }
            }
        }

        #endregion
    }
}
