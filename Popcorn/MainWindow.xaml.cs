using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using Popcorn.ViewModel;
using Popcorn.Helpers;
using Popcorn.Events;

namespace Popcorn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var vm = DataContext as MainViewModel;
            if (vm == null)
                return;

            vm.ConnectionError += OnConnectionInError;
            vm.WindowStageChanged += OnWindowStateChanged;
        }

        #endregion

        #region Methods

        #region Method -> OnConnectionInError

        /// <summary>
        /// Open a popup when a connection error has occured
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private async void OnConnectionInError(object sender, ConnectionErrorEventArgs e)
        {
            var settings = new MetroDialogSettings
            {
                ColorScheme = MetroDialogColorScheme.Theme
            };

            var result = await
                this.ShowMessageAsync(LocalizationProviderHelper.GetLocalizedValue<string>("ConnectionErrorTitlePopup"),
                    LocalizationProviderHelper.GetLocalizedValue<string>("ConnectionErrorDescriptionPopup"),
                    MessageDialogStyle.Affirmative, settings);
            if (result != MessageDialogResult.Affirmative)
                return;

            // Close the movie page
            if (MoviePage.IsOpen)
            {
                MoviePage.IsOpen = false;
            }
        }

        #endregion

        #region Method -> OnWindowStateChanged

        /// <summary>
        /// When window got maximized while a movie is playing : collapse menu bar, header tab and let tabcontrol takes up all the place available. 
        /// Rollback when window go back to normal.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnWindowStateChanged(object sender, WindowStateChangedEventArgs e)
        {
            if (e.IsMoviePlaying)
            {
                SearchBar.Visibility = Visibility.Collapsed;
                MenuBar.Visibility = Visibility.Collapsed;
                Grid.SetRow(MainTabControl, 0);
                Grid.SetRowSpan(MainTabControl, 2);
                Grid.SetColumn(MainTabControl, 0);
                Grid.SetColumnSpan(MainTabControl, 2);
                var headerPanelScroll = MainTabControl.FindChild<ScrollViewer>("HeaderPanelScroll");
                headerPanelScroll.Visibility = Visibility.Collapsed;
                UseNoneWindowStyle = true;
            }
            else
            {
                SearchBar.Visibility = Visibility.Visible;
                MenuBar.Visibility = Visibility.Visible;
                Grid.SetRow(MainTabControl, 0);
                Grid.SetRowSpan(MainTabControl, 2);
                Grid.SetColumn(MainTabControl, 1);
                Grid.SetColumnSpan(MainTabControl, 1);
                var headerPanelScroll = MainTabControl.FindChild<ScrollViewer>("HeaderPanelScroll");
                headerPanelScroll.Visibility = Visibility.Visible;
                UseNoneWindowStyle = false;
            }
        }

        #endregion

        #endregion
    }
}