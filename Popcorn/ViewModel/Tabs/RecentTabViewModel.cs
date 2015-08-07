using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Comparers;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

namespace Popcorn.ViewModel.Tabs
{
    /// <summary>
    /// The recent movies tab
    /// </summary>
    public class RecentTabViewModel : TabsViewModel
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public RecentTabViewModel()
        {
            TabName = LocalizationProviderHelper.GetLocalizedValue<string>("RecentTitleTab");

            Messenger.Default.Register<ChangeLanguageMessage>(
                this,
                language =>
                {
                    TabName = LocalizationProviderHelper.GetLocalizedValue<string>("RecentTitleTab");
                });

            // Reload movies
            ReloadMovies = new RelayCommand(async () =>
            {
                await LoadNextPageAsync();
                Messenger.Default.Send(new ConnectionErrorMessage(string.Empty, true));
            });

            if (!Movies.Any())
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () => await LoadNextPageAsync());
            }
        }

        #endregion

        #region Method -> LoadNextPageAsync

        /// <summary>
        /// Load next page with an optional search parameter
        /// </summary>
        public async Task LoadNextPageAsync()
        {
            Page++;
            IsLoadingMovies = true;
            try
            {
                var movieResults =
                    await ApiService.GetRecentMoviesAsync(Page,
                        MaxMoviesPerPage,
                        CancellationLoadNextPageToken.Token);
                var movies = movieResults.ToList();

                foreach (var movie in movies)
                {
                    Movies.Add(movie);
                }

                await ApiService.DownloadCoverImageAsync(movies);
                await UserDataService.ComputeMovieHistoryAsync(movies);

                IsLoadingMovies = false;

                if (!Movies.Any() && !movies.Any())
                {
                    IsMovieFound = false;
                }
                else
                {
                    IsMovieFound = true;
                }
            }
            catch
            {
                IsLoadingMovies = false;
                IsMovieFound = Movies.Any();
                Page--;
            }
        }

        #endregion
    }
}