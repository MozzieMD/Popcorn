using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using GalaSoft.MvvmLight.CommandWpf;

namespace Popcorn.ViewModel.Tabs
{
    /// <summary>
    /// The recent movies tab
    /// </summary>
    public sealed class RecentTabViewModel : TabsViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RecentTabViewModel class.
        /// </summary>
        private RecentTabViewModel()
        {
            RegisterMessages();
            RegisterCommands();
            TabName = LocalizationProviderHelper.GetLocalizedValue<string>("RecentTitleTab");
        }

        #endregion

        #region Methods

        #region Method -> InitializeAsync
        /// <summary>
        /// Load asynchronously the recent movies for the current instance
        /// </summary>
        /// <returns>Instance of RecentTabViewModel</returns>
        private async Task<RecentTabViewModel> InitializeAsync()
        {
            await LoadNextPageAsync();
            return this;
        }
        #endregion

        #region Method -> CreateAsync
        /// <summary>
        /// Initialize asynchronously an instance of the RecentTabViewModel class
        /// </summary>
        /// <returns>Instance of RecentTabViewModel</returns>
        public static Task<RecentTabViewModel> CreateAsync()
        {
            var ret = new RecentTabViewModel();
            return ret.InitializeAsync();
        }
        #endregion

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<ChangeLanguageMessage>(
                this,
                language => { TabName = LocalizationProviderHelper.GetLocalizedValue<string>("RecentTitleTab"); });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            ReloadMovies = new RelayCommand(async () =>
            {
                await LoadNextPageAsync();
                Messenger.Default.Send(new ConnectionErrorMessage(string.Empty, true));
            });
        }

        #endregion

        #region Method -> LoadNextPageAsync

        /// <summary>
        /// Load next page
        /// </summary>
        public async Task LoadNextPageAsync()
        {
            Page++;
            IsLoadingMovies = true;
            try
            {
                var movieResults =
                    await MovieService.GetRecentMoviesAsync(Page,
                        MaxMoviesPerPage,
                        CancellationLoadNextPageToken.Token);
                var movies = movieResults.ToList();

                foreach (var movie in movies)
                {
                    Movies.Add(movie);
                }

                await UserDataService.ComputeMovieHistoryAsync(movies);
                await MovieService.DownloadCoverImageAsync(movies);
            }
            catch
            {
                Page--;
            }
            finally
            {
                IsLoadingMovies = false;
                IsMovieFound = Movies.Any();
            }
        }

        #endregion

        #endregion
    }
}