using System;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using Popcorn.Messaging;
using Popcorn.Events;
using Popcorn.ViewModel.Tabs;
using Popcorn.ViewModel.Players;

namespace Popcorn.ViewModel
{
    /// <summary>
    /// Main applcation's viewmodel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> Tabs

        private ObservableCollection<ITab> _tabs;

        /// <summary>
        /// Tabs shown into the interface via TabControl
        /// </summary>
        public ObservableCollection<ITab> Tabs
        {
            get { return _tabs; }
            set { Set(() => Tabs, ref _tabs, value); }
        }

        #endregion

        #region Property -> SelectedTab

        private ITab _selectedTab;

        /// <summary>
        /// The selected viewmodel tab via TabControl
        /// </summary>
        public ITab SelectedTab
        {
            get { return _selectedTab; }
            set { Set(() => SelectedTab, ref _selectedTab, value); }
        }

        #endregion

        #region Property -> IsConnectionInError

        private bool _isConnectionInError;

        /// <summary>
        /// Specify if a connection error has occured
        /// </summary>
        public bool IsConnectionInError
        {
            get { return _isConnectionInError; }
            set { Set(() => IsConnectionInError, ref _isConnectionInError, value); }
        }

        #endregion

        #region Property -> IsSettingsFlyoutOpen

        private bool _isSettingsFlyoutOpen;

        /// <summary>
        /// Specify if settings flyout is open
        /// </summary>
        public bool IsSettingsFlyoutOpen
        {
            get { return _isSettingsFlyoutOpen; }
            set { Set(() => IsSettingsFlyoutOpen, ref _isSettingsFlyoutOpen, value); }
        }

        #endregion

        #region Property -> IsMovieFlyoutOpen

        private bool _isMovieFlyoutOpen;

        /// <summary>
        /// Specify if movie flyout is open
        /// </summary>
        public bool IsMovieFlyoutOpen
        {
            get { return _isMovieFlyoutOpen; }
            set { Set(() => IsMovieFlyoutOpen, ref _isMovieFlyoutOpen, value); }
        }

        #endregion

        #endregion

        #region Commands

        #region Command -> CloseMoviePageCommand

        /// <summary>
        /// Command used to close movie page
        /// </summary>
        public RelayCommand CloseMoviePageCommand { get; private set; }

        #endregion

        #region Command -> MainWindowClosingCommand

        /// <summary>
        /// Command used to close the application
        /// </summary>
        public RelayCommand MainWindowClosingCommand { get; private set; }

        #endregion

        #region Command -> OpenSettingsCommand

        /// <summary>
        /// Command used to open application settings
        /// </summary>
        public RelayCommand OpenSettingsCommand { get; private set; }

        #endregion

        #endregion

        #region Constructors

        #region Constructor -> MainViewModel

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            RegisterMessages();
            RegisterCommands();

            Tabs = new ObservableCollection<ITab>
            {
                new PopularTabViewModel(),
                new GreatestTabViewModel(),
                new RecentTabViewModel()
            };

            SelectedTab = Tabs.FirstOrDefault();
        }

        #endregion

        #endregion

        #region Methods 

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<ConnectionErrorMessage>(this, e =>
            {
                if (!e.ResetConnectionError)
                {
                    IsConnectionInError = true;
                    OnConnectionError(new ConnectionErrorEventArgs(e.Message));
                }
                else
                {
                    IsConnectionInError = !e.ResetConnectionError;
                }
            });

            Messenger.Default.Register<LoadMovieMessage>(this, e => { IsMovieFlyoutOpen = true; });

            Messenger.Default.Register<MovieBufferedMessage>(this, message =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    // Create a tab with the movie name as the title
                    Tabs.Add(new MoviePlayerViewModel(message.Movie, message.MovieUri));

                    // Select this tab in the tab control
                    SelectedTab = Tabs.Last();

                    // Open movie flyout
                    IsMovieFlyoutOpen = false;
                });
            });

            // Inform subscribers that a movie has stopped playing
            Messenger.Default.Register<StopPlayingMovieMessage>(
                this,
                message =>
                {
                    // Remove the movie tab
                    MoviePlayerViewModel moviePlayer = null;
                    foreach (var tab in Tabs)
                    {
                        var mediaViewModel = tab as MoviePlayerViewModel;
                        if (mediaViewModel != null)
                        {
                            moviePlayer = mediaViewModel;
                        }
                    }
                    if (moviePlayer != null)
                    {
                        Tabs.Remove(moviePlayer);
                        moviePlayer.Cleanup();
                        SelectedTab = Tabs.FirstOrDefault();
                    }

                    IsMovieFlyoutOpen = true;
                });

            Messenger.Default.Register<ChangeScreenModeMessage>(
                this,
                message =>
                {
                    if (message.IsFullScreen)
                    {
                        OnToggleFullScreen(new EventArgs());
                    }
                    else
                    {
                        OnBackToNormalScreenMode(new EventArgs());
                    }
                });

            Messenger.Default.Register<SearchMovieMessage>(this,
                async message => { await SearchMovies(message.Filter); });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            CloseMoviePageCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new ChangeScreenModeMessage(false));
                Messenger.Default.Send(new StopPlayingTrailerMessage());
            });

            MainWindowClosingCommand = new RelayCommand(() =>
            {
                foreach (var tab in Tabs)
                {
                    var moviesViewModelTab = tab as TabsViewModel;
                    moviesViewModelTab?.Cleanup();
                }

                ViewModelLocator.Cleanup();
            });

            OpenSettingsCommand = new RelayCommand(() => { IsSettingsFlyoutOpen = true; });
        }

        #endregion

        #region Method -> SearchMovies

        /// <summary>
        /// Search for movie with a criteria
        /// </summary>
        /// <param name="criteria">The criteria used for search</param>
        private async Task SearchMovies(string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
            {
                // The search filter is empty. We have to find the search tab if any
                foreach (var tab in Tabs)
                {
                    var searchTab = tab as SearchTabViewModel;
                    if (searchTab != null)
                    {
                        // We've found it
                        var searchTabToRemove = searchTab;
                        // The search tab is currently selected in the UI, we have to pick a different selected tab prior deleting
                        if (searchTabToRemove == SelectedTab)
                        {
                            SelectedTab = Tabs.FirstOrDefault();
                        }

                        // Remove the search tab
                        Tabs.Remove(searchTabToRemove);
                        searchTabToRemove.Cleanup();
                        return;
                    }
                }
            }
            else
            {
                foreach (var tab in Tabs)
                {
                    // Looking for a Search tab. If any, search movies with the criteria, and select this tab to be shown in the UI
                    var searchTab = tab as SearchTabViewModel;
                    if (searchTab != null)
                    {
                        await searchTab.SearchMoviesAsync(criteria);

                        if (SelectedTab != searchTab)
                        {
                            SelectedTab = searchTab;
                        }

                        return;
                    }
                }

                // There is no current search tab, we have to create it
                Tabs.Add(new SearchTabViewModel());

                // Select in the UI the search tab we've just created
                SelectedTab = Tabs.Last();

                // Search movies with criteria
                var searchMovieTab = SelectedTab as SearchTabViewModel;
                if (searchMovieTab != null)
                {
                    await searchMovieTab.SearchMoviesAsync(criteria);
                }
            }
        }

        #endregion

        #endregion

        #region Events

        #region Event -> OnConnectionError

        /// <summary>
        /// Fired  on connection error
        /// </summary>
        public event EventHandler<ConnectionErrorEventArgs> ConnectionError;

        /// <summary>
        /// Fire ConnectionError
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnConnectionError(ConnectionErrorEventArgs e)
        {
            var handler = ConnectionError;
            handler?.Invoke(this, e);
        }

        #endregion

        #region Event -> OnToggleFullScreen

        /// <summary>
        /// Fired when fullscreen mode has been requested
        /// </summary>
        public event EventHandler<EventArgs> ToggleFullScreenChanged;

        /// <summary>
        /// Fire event ToggleFullScreenChanged
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnToggleFullScreen(EventArgs e)
        {
            var handler = ToggleFullScreenChanged;
            handler?.Invoke(this, e);
        }

        #endregion

        #region Event -> OnBackToNormalScreenMode

        /// <summary>
        /// Fired when back to normal screen mode has been requested
        /// </summary>
        public event EventHandler<EventArgs> BackToNormalScreenMode;

        /// <summary>
        /// Fire event BackToNormalScreenMode
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnBackToNormalScreenMode(EventArgs e)
        {
            var handler = BackToNormalScreenMode;
            handler?.Invoke(this, e);
        }

        #endregion

        #endregion
    }
}