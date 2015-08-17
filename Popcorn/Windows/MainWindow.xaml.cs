using System.Windows;
using System.Windows.Controls;
using Popcorn.Helpers;
using Popcorn.Events;
using Popcorn.ViewModels.Main;

namespace Popcorn.Windows
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

            vm.WindowStageChanged += OnWindowStateChanged;
        }

        #endregion

        #region Methods

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
                Grid.SetRowSpan(MainTabControl, 4);
                Grid.SetColumn(MainTabControl, 0);
                Grid.SetColumnSpan(MainTabControl, 3);
                var headerPanelScroll = MainTabControl.FindChild<ScrollViewer>("HeaderPanelScroll");
                headerPanelScroll.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchBar.Visibility = Visibility.Visible;
                MenuBar.Visibility = Visibility.Visible;
                Grid.SetRow(MainTabControl, 1);
                Grid.SetRowSpan(MainTabControl, 3);
                Grid.SetColumn(MainTabControl, 1);
                Grid.SetColumnSpan(MainTabControl, 1);
                var headerPanelScroll = MainTabControl.FindChild<ScrollViewer>("HeaderPanelScroll");
                headerPanelScroll.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #endregion
    }
}