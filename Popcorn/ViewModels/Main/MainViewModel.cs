using System;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls.Dialogs;
using Popcorn.Dialogs;
using Popcorn.Messaging;
using Popcorn.Events;
using Popcorn.Helpers;
using Popcorn.Services.User;
using Popcorn.ViewModels.Tabs;
using Popcorn.ViewModels.Players.Movie;

namespace Popcorn.ViewModels.Main
{
    /// <summary>
    /// Main applcation's viewmodel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Properties

        #region Property -> UserService

        /// <summary>
        /// Used to interacts with user
        /// </summary>
        private UserService UserService { get; }

        #endregion

        #region Property -> IDialogCoordinator

        /// <summary>
        /// Used to define the dialog context
        /// </summary>
        private IDialogCoordinator DialogCoordinator { get; }

        #endregion

        #region Property -> IsStarting

        private bool _isStarting;

        /// <summary>
        /// Indicates if the application is starting
        /// </summary>
        public bool IsStarting
        {
            get { return _isStarting; }
            set { Set(() => IsStarting, ref _isStarting, value); }
        }

        #endregion

        #region Property -> IsUserSignin

        private bool _isUserSignin;

        /// <summary>
        /// Indicates if the user is signed in
        /// </summary>
        public bool IsUserSignin
        {
            get { return _isUserSignin; }
            set { Set(() => IsUserSignin, ref _isUserSignin, value); }
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

        #region Property -> IsManagingException

        private bool _isManagingException;

        /// <summary>
        /// Indicates if an exception is currently managed
        /// </summary>
        private bool IsManagingException
        {
            get { return _isManagingException; }
            set
            {
                Set(() => IsManagingException, ref _isManagingException, value);
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
            private set { Set(() => IsPopularTabSelected, ref _isPopularTabSelected, value); }
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
            private set { Set(() => IsGreatestTabSelected, ref _isGreatestTabSelected, value); }
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
            private set { Set(() => IsRecentTabSelected, ref _isRecentTabSelected, value); }
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
            private set { Set(() => IsSearchTabSelected, ref _isSearchTabSelected, value); }
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
            set { Set(() => IsMovieSearchActive, ref _isMovieSearchActive, value); }
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

        #region Command -> ShowLoginDialogCommand

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
                    await OpenSigninModal();
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
        public MainViewModel() :
            this(MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private MainViewModel(IDialogCoordinator dialogCoordinator)
        {
            UserService = SimpleIoc.Default.GetInstance<UserService>();
            RegisterMessages();
            RegisterCommands();
            DialogCoordinator = dialogCoordinator;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
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
            Messenger.Default.Register<ManageExceptionMessage>(this, e =>
            {
                ManageException(e.UnHandledException);
            });

            Messenger.Default.Register<LoadMovieMessage>(this, e => { IsMovieFlyoutOpen = true; });

            Messenger.Default.Register<PlayMovieMessage>(this, message =>
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
                    if (!IsStarting)
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

        #region Method -> OpenSigninModal

        /// <summary>
        /// Open signin modal
        /// </summary>
        /// <returns>Task</returns>
        private async Task OpenSigninModal()
        {
            var signinDialog = new SigninDialog(new SigninDialogSettings());
            await DialogCoordinator.ShowMetroDialogAsync(this, signinDialog);
            var signinDialogResult = await signinDialog.WaitForButtonPressAsync();
            await DialogCoordinator.HideMetroDialogAsync(this, signinDialog);
            if (signinDialogResult == null)
                return;

            if (signinDialogResult.ShouldSignup)
            {
                var user = await OpenSignupModal();
                if (user == null)
                    return;
                await Signin(user);
            }
            else
            {
                var user = new Models.Account.User
                {
                    Username = signinDialogResult.Username,
                    Password = signinDialogResult.Password
                };

                await Signin(user);
            }
        }

        #endregion

        #region Method -> OpenSignupModal

        /// <summary>
        /// Open signup modal
        /// </summary>
        /// <returns>User</returns>
        private async Task<Models.Account.User> OpenSignupModal()
        {
            var signupDialog = new SignupDialog(new SignupDialogSettings());
            await DialogCoordinator.ShowMetroDialogAsync(this, signupDialog);
            var signupDialogResult = await signupDialog.WaitForButtonPressAsync();
            await DialogCoordinator.HideMetroDialogAsync(this, signupDialog);
            if (signupDialogResult == null)
                return null;
            return await
                UserService.CreateUser(signupDialogResult.Username, signupDialogResult.FirstName,
                    signupDialogResult.LastName, signupDialogResult.Password, signupDialogResult.Email,
                    new CancellationToken());
        }

        #endregion

        #region Method -> Signin

        /// <summary>
        /// Signin user
        /// </summary>
        /// <param name="user">The user to signin</param>
        /// <returns>Task</returns>
        private async Task Signin(Models.Account.User user)
        {
            var bearer = await UserService.Signin(user, new CancellationToken());
        }

        #endregion

        #region Method -> OnUnhandledException

        /// <summary>
        /// Display a dialog on unhandled exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                ManageException(ex);
            }
        }

        #endregion

        #region Method -> ManageException

        /// <summary>
        /// Manage an exception
        /// </summary>
        /// <param name="exception">The exception</param>
        private void ManageException(Exception exception)
        {
            if (IsManagingException)
                return;

            IsManagingException = true;
            if (exception is WebException || exception is SocketException)
                IsConnectionInError = true;

            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                var exceptionDialog = new ExceptionDialog(new ExceptionDialogSettings(LocalizationProviderHelper.GetLocalizedValue<string>("EmbarrassingError"), exception.Message));
                await DialogCoordinator.ShowMetroDialogAsync(this, exceptionDialog);
                await exceptionDialog.WaitForButtonPressAsync();
                IsManagingException = false;
                await DialogCoordinator.HideMetroDialogAsync(this, exceptionDialog);
            });
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