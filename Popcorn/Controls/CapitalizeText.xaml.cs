using System.Threading;
using System.Windows;

namespace Popcorn.Controls
{
    /// <summary>
    /// Interaction logic for CapitalizeText.xaml
    /// </summary>
    public partial class CapitalizeText
    {
        #region DependencyProperties

        #region DependencyPropertiy -> RuntimeProperty

        /// <summary>
        /// TextProperty
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
                typeof(string), typeof(CapitalizeText),
                new PropertyMetadata(string.Empty, OnTextChanged));

        #endregion

        #endregion

        #region Properties

        #region Property -> Text

        /// <summary>
        /// The text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of CapitalizeText
        /// </summary>
        public CapitalizeText()
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
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var capitalizeText = d as CapitalizeText;
            capitalizeText?.DisplayCapitalizedText();
        }

        #endregion

        #region Method -> DisplayCapitalizedText

        /// <summary>
        /// Display capitalized text
        /// </summary>
        private void DisplayCapitalizedText()
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            DisplayText.Text = textInfo.ToTitleCase(Text);
        }

        #endregion
    }
}
