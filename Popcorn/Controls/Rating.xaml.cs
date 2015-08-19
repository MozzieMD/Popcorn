using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Popcorn.Controls
{
    /// <summary>
    /// Interaction logic for Rating.xaml
    /// </summary>
    public partial class Rating
    {
        public static readonly DependencyProperty RatingValueProperty = DependencyProperty.Register("RatingValue",
            typeof (int), typeof (Rating),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RatingChanged));

        private const int Max = 5;

        public int RatingValue
        {
            get { return (int) GetValue(RatingValueProperty); }
            set
            {
                if (value < 0)
                {
                    SetValue(RatingValueProperty, 0);
                }
                else if (value > Max)
                {
                    SetValue(RatingValueProperty, Max);
                }
                else
                {
                    SetValue(RatingValueProperty, value);
                }
            }
        }

        private static void RatingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var rating = sender as Rating;
            if (rating == null)
            {
                return;
            }

            var newval = (int) e.NewValue;
            var childs = ((Grid) (rating.Content)).Children;

            ToggleButton button;

            for (var i = 0; i < newval; i++)
            {
                button = childs[i] as ToggleButton;
                if (button != null)
                    button.IsChecked = true;
            }

            for (var i = newval; i < childs.Count; i++)
            {
                button = childs[i] as ToggleButton;
                if (button != null)
                    button.IsChecked = false;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rating()
        {
            InitializeComponent();
        }
    }
}