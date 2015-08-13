using System;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls.Dialogs;
using Popcorn.CustomDialogs;
using Popcorn.Messaging;
using Popcorn.Events;
using Popcorn.ViewModel.Tabs;
using Popcorn.ViewModel.Players.Movie;

namespace Popcorn.ViewModel
{
    /// <summary>
    /// Main applcation's viewmodel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> IDialogCoordinator

        /// <summary>
        /// Used to define the dialog context
        /// </summary>
        private readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Property -> IsStarting

        private bool _isStarting;

        /// <summary>
        /// Indicates if the application is starting
        /// </summary>
        public bool IsStarting
        {
            get { return _isStarting; }
            set
            {
                Set(() => IsStarting, ref _isStarting, value);
            }
        }

        #endregion

        #region Property -> IsMoviePlaying

        private bool _isMoviePlaying;

        /// <summary>
        /// Indicates if a movie is playing
        /// </summary>
        private bool IsMoviePlaying
        {
            get { return _isMoviePlaying; }
            set
            {
                Set(() => IsMoviePlaying, ref _isMoviePlaying, value);
                OnWindowStateChanged(new WindowStateChangedEventArgs(value));
            }
        }

        #endregion

        #region Property -> IsFullScreen

        private bool _isFullScreen;

        /// <summary>
        /// Indicates if application is fullscreen
        /// </summary>
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                Set(() => IsFullScreen, ref _isFullScreen, value);
                OnWindowStateChanged(new WindowStateChangedEventArgs(IsMoviePlaying));
            }
        }

        #endregion

        #region Property -> Tabs

        private ObservableCollection<ITab> _tabs = new ObservableCollection<ITab>();

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

        #region Property -> IsPopularTabSelected

        private bool _isPopularTabSelected;

        /// <summary>
        /// Indicates if the popular movies tab is selected
        /// </summary>
        public bool IsPopularTabSelected
        {
            get { return _isPopularTabSelected; }
            set
            {
                Set(() => IsPopularTabSelected, ref _isPopularTabSelected, value);
            }
        }

        #endregion

        #region Property -> IsGreatestTabSelected

        private bool _isGreatestTabSelected;

        /// <summary>
        /// Indicates if the greatest movies tab is selected
        /// </summary>
        public bool IsGreatestTabSelected
        {
            get { return _isGreatestTabSelected; }
            set
            {
                Set(() => IsGreatestTabSelected, ref _isGreatestTabSelected, value);
            }
        }

        #endregion

        #region Property -> IsRecentTabSelected

        private bool _isRecentTabSelected;

        /// <summary>
        /// Indicates if the recent movies tab is selected
        /// </summary>
        public bool IsRecentTabSelected
        {
            get { return _isRecentTabSelected; }
            set
            {
                Set(() => IsRecentTabSelected, ref _isRecentTabSelected, value);
            }
        }

        #endregion

        #region Property -> IsSearchTabSelected

        private bool _isSearchTabSelected;

        /// <summary>
        /// Indicates if the search movies tab is selected
        /// </summary>
        public bool IsSearchTabSelected
        {
            get { return _isSearchTabSelected; }
            set
            {
                Set(() => IsSearchTabSelected, ref _isSearchTabSelected, value);
            }
        }

        #endregion

        #region Property -> IsMovieSearchActive

        private bool _isMovieSearchActive;

        /// <summary>
        /// Indicates if a movie search is active
        /// </summary>
        public bool IsMovieSearchActive
        {
            get { return _isMovieSearchActive; }
            set
            {
                Set(() => IsMovieSearchActive, ref _isMovieSearchActive, value);
            }
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

        #region Command -> SelectGreatestTab

        /// <summary>
        /// Command used to select the greatest movies tab
        /// </summary>
        public RelayCommand SelectGreatestTab { get; private set; }

        #endregion

        #region Command -> SelectPopularTab

        /// <summary>
        /// Command used to select the popular movies tab
        /// </summary>
        public RelayCommand SelectPopularTab { get; private set; }

        #endregion

        #region Command -> SelectRecentTab

        /// <summary>
        /// Command used to select the recent movies tab
        /// </summary>
        public RelayCommand SelectRecentTab { get; private set; }

        #endregion

        #region Command -> SelectSearchTab

        /// <summary>
        /// Command used to select the search movies tab
        /// </summary>
        public RelayCommand SelectSearchTab { get; private set; }

        #endregion

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

        #region Command -> InitializeAsyncCommand

        /// <summary>
        /// Command used to load tabs
        /// </summary>
        public RelayCommand InitializeAsyncCommand { get; private set; }

        #endregion

        #region Command -> InitializeAsyncCommand

        private RelayCommand _showLoginDialogCommand;

        /// <summary>
        /// Show login dialog
        /// </summary>
        public RelayCommand ShowLoginDialogCommand
        {
            get
            {
                return _showLoginDialogCommand ?? (_showLoginDialogCommand = new RelayCommand(async () =>
                {
                    var customDialog = new SigninDialog(new SigninDialogSettings());
                    await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
                    var result = await customDialog.WaitForButtonPressAsync();
                    await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                }));
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region Constructor -> MainViewModel

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel():
            this(DialogCoordinator.Instance)
        { }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private MainViewModel(IDialogCoordinator dialogCoordinator)
        {
            RegisterMessages();
            RegisterCommands();
            _dialogCoordinator = dialogCoordinator;
        }

        #endregion

        #endregion

        #region Methods 

        #region Method -> InitializeAsync

        /// <summary>
        /// Load asynchronously the languages of the application and return an instance of MainViewModel
        /// </summary>
        /// <returns>Instance of MainViewModel</returns>
        private async Task InitializeAsync()
        {
            IsStarting = true;
            Tabs.Add(await PopularTabViewModel.CreateAsync());
            SelectedTab = Tabs.FirstOrDefault();
            IsGreatestTabSelected = false;
            IsPopularTabSelected = true;
            IsRecentTabSelected = false;
            IsSearchTabSelected = false;
            IsStarting = false;
            Tabs.Add(await GreatestTabViewModel.CreateAsync());
            Tabs.Add(await RecentTabViewModel.CreateAsync());
        }

        #endregion

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
                    //DialogCoordinator.Instance.ShowMessageAsync(this, "Message from VM", "MVVM based messages!").ContinueWith(t => Console.WriteLine(t.Result));
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
                    Tabs.Add(new MoviePlayerViewModel(message.Movie));
                    SelectedTab = Tabs.Last();
                    IsMovieFlyoutOpen = false;
                    IsMoviePlaying = true;
                });
            });

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
                    IsMoviePlaying = false;
                });

            Messenger.Default.Register<SearchMovieMessage>(this,
                async message =>
                {
                    if(!IsStarting)
                        await SearchMovies(message.Filter);
                });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            SelectGreatestTab = new RelayCommand(() =>
            {
                if (SelectedTab is GreatestTabViewModel)
                    return;
                foreach (var tab in Tabs)
                {
                    var greatestTab = tab as GreatestTabViewModel;
                    if (greatestTab != null)
                    {
                        SelectedTab = greatestTab;
                        IsGreatestTabSelected = true;
                        IsPopularTabSelected = false;
                        IsRecentTabSelected = false;
                        IsSearchTabSelected = false;
                    }
                }
            });

            SelectPopularTab = new RelayCommand(() =>
            {
                if (SelectedTab is PopularTabViewModel)
                    return;
                foreach (var tab in Tabs)
                {
                    var popularTab = tab as PopularTabViewModel;
                    if (popularTab != null)
                    {
                        SelectedTab = popularTab;
                        IsGreatestTabSelected = false;
                        IsPopularTabSelected = true;
                        IsRecentTabSelected = false;
                        IsSearchTabSelected = false;
                    }
                }
            });

            SelectRecentTab = new RelayCommand(() =>
            {
                if (SelectedTab is RecentTabViewModel)
                    return;
                foreach (var tab in Tabs)
                {
                    var recentTab = tab as RecentTabViewModel;
                    if (recentTab != null)
                    {
                        SelectedTab = recentTab;
                        IsGreatestTabSelected = false;
                        IsPopularTabSelected = false;
                        IsRecentTabSelected = true;
                        IsSearchTabSelected = false;
                    }
                }
            });

            SelectSearchTab = new RelayCommand(() =>
            {
                if (SelectedTab is SearchTabViewModel)
                    return;
                foreach (var tab in Tabs)
                {
                    var searchTab = tab as SearchTabViewModel;
                    if (searchTab != null)
                    {
                        SelectedTab = searchTab;
                        IsGreatestTabSelected = false;
                        IsPopularTabSelected = false;
                        IsRecentTabSelected = false;
                        IsSearchTabSelected = true;
                    }
                }
            });

            CloseMoviePageCommand = new RelayCommand(() =>
            {
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

            InitializeAsyncCommand = new RelayCommand(async () => await InitializeAsync());
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

                        Tabs.Remove(searchTabToRemove);
                        searchTabToRemove.Cleanup();
                        IsMovieSearchActive = false;
                        IsGreatestTabSelected = false;
                        IsPopularTabSelected = true;
                        IsRecentTabSelected = false;
                        IsSearchTabSelected = false;
                        return;
                    }
                }
            }
            else
            {
                IsMovieSearchActive = true;
                IsGreatestTabSelected = false;
                IsPopularTabSelected = false;
                IsRecentTabSelected = false;
                IsSearchTabSelected = true;

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

                Tabs.Add(new SearchTabViewModel());
                SelectedTab = Tabs.Last();
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

        public event EventHandler<ConnectionErrorEventArgs> ConnectionError;

        /// <summary>
        /// Fire on connection error
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnConnectionError(ConnectionErrorEventArgs e)
        {
            var handler = ConnectionError;
            handler?.Invoke(this, e);
        }

        #endregion

        #region Event -> OnWindowStateChanged

        public event EventHandler<WindowStateChangedEventArgs> WindowStageChanged;

        /// <summary>
        /// Fire when window state has changed
        /// </summary>
        ///<param name="e">Event data</param>
        private void OnWindowStateChanged(WindowStateChangedEventArgs e)
        {
            var handler = WindowStageChanged;
            handler?.Invoke(this, e);
        }

        #endregion

        #endregion
    }
}