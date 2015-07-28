using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using Popcorn.ViewModel;
using Popcorn.Helpers;
using Popcorn.Events;
using Popcorn.ViewModel.Tabs;

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

            Loaded += OnLoaded;
            Closing += OnClosing;
        }

        #endregion

        #region Methods

        #region Method -> OnClosing
        /// <summary>
        /// Do actions when closing
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CancelEventArgs</param>
        private void OnClosing(object sender, CancelEventArgs e)
        {
            Loaded -= OnLoaded;

            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                vm.ConnectionError -= OnConnectionInError;
                vm.ToggleFullScreenChanged -= OnToggleFullScreen;
                vm.BackToNormalScreenMode -= OnBackToNormalScreen;

                foreach (var tab in vm.Tabs)
                {
                    var moviesViewModelTab = tab as TabsViewModel;
                    moviesViewModelTab?.Cleanup();
                }
            }            

            ViewModelLocator.Cleanup();
        }
        #endregion

        #region Method -> OnLoaded
        /// <summary>
        /// Do actions when loaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">RoutedEventArgs</param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                vm.ConnectionError += OnConnectionInError;
                vm.ToggleFullScreenChanged += OnToggleFullScreen;
                vm.BackToNormalScreenMode += OnBackToNormalScreen;
            }
        }
        #endregion  

        #region Method -> OnConnectionInError

        /// <summary>
        /// Open the popup when a connection error has occured
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private async void OnConnectionInError(object sender, ConnectionErrorEventArgs e)
        {
            // Set and open a MetroDialog to inform that a connection error occured
            var settings = new MetroDialogSettings();
            settings.ColorScheme = MetroDialogColorScheme.Theme;
            var result = await
                this.ShowMessageAsync(LocalizationProviderHelper.GetLocalizedValue<string>("ConnectionErrorTitlePopup"),
                    LocalizationProviderHelper.GetLocalizedValue<string>("ConnectionErrorDescriptionPopup"),
                    MessageDialogStyle.Affirmative, settings);

            if (result == MessageDialogResult.Affirmative)
            {
                // Close the movie page
                if (MoviePage.IsOpen)
                {
                    MoviePage.IsOpen = false;
                }
            }
        }

        #endregion

        #region Method -> OnToggleFullScreen
        /// <summary>
        /// On toggle fullscreen, maximize the main window, collapse menu bar, header tab and let tabcontrol takes up all the place available
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnToggleFullScreen(object sender, EventArgs e)
        {
            SearchBar.Visibility = Visibility.Collapsed;
            MenuBar.Visibility = Visibility.Collapsed;
            Grid.SetRow(MainTabControl, 0);
            Grid.SetRowSpan(MainTabControl, 2);
            Grid.SetColumn(MainTabControl, 0);
            Grid.SetColumnSpan(MainTabControl, 2);
            var headerPanelScroll = VisualHierarchyHelper.FindChild<ScrollViewer>(MainTabControl, "HeaderPanelScroll");
            headerPanelScroll.Visibility = Visibility.Collapsed;
            UseNoneWindowStyle = true;
            WindowState = WindowState.Maximized;
        }
        #endregion

        #region Method -> OnBackToNormalScreen
        /// <summary>
        /// On back to normal screen, go back to a normal sized window, show menu bar, header tab and let tabcontrol takes its original place
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnBackToNormalScreen(object sender, EventArgs e)
        {
            SearchBar.Visibility = Visibility.Visible;
            MenuBar.Visibility = Visibility.Visible;
            Grid.SetRow(MainTabControl, 0);
            Grid.SetRowSpan(MainTabControl, 2);
            Grid.SetColumn(MainTabControl, 1);
            Grid.SetColumnSpan(MainTabControl, 1);
            var headerPanelScroll = VisualHierarchyHelper.FindChild<ScrollViewer>(MainTabControl, "HeaderPanelScroll");
            headerPanelScroll.Visibility = Visibility.Visible;
            UseNoneWindowStyle = false;
            WindowState = WindowState.Normal;
        }
        #endregion

        #endregion
    }
}