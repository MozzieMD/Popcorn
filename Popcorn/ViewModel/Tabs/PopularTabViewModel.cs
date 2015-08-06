using System;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Comparers;
using GalaSoft.MvvmLight.Threading;

namespace Popcorn.ViewModel.Tabs
{
    /// <summary>
    /// The popular movies tab
    /// </summary>
    public class PopularTabViewModel : TabsViewModel
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PopularTabViewModel()
        {
            TabName = LocalizationProviderHelper.GetLocalizedValue<string>("PopularTitleTab");

            Messenger.Default.Register<ChangeLanguageMessage>(
            this,
            language =>
            {
                TabName = LocalizationProviderHelper.GetLocalizedValue<string>("PopularTitleTab");
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
                    await ApiService.GetPopularMoviesAsync(Page,
                        MaxMoviesPerPage,
                        CancellationLoadNextPageToken.Token);
                var movies = movieResults.ToList();

                // Now we download the cover image for each movie
                foreach (var movie in movies.Except(Movies, new MovieComparer()))
                {
                    // Download the poster image of the movie
                    movie.CoverImagePath =
                        await ApiService.DownloadCoverImageAsync(movie.ImdbCode,
                            new Uri(movie.MediumCoverImage),
                            CancellationLoadNextPageToken.Token);

                    Movies.Add(movie);
                }

                await UserDataService.ComputeMovieHistoryAsync(Movies);

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
            catch (Exception)
            {
                IsLoadingMovies = false;
                IsMovieFound = Movies.Any();
                Page--;
            }
        }

        #endregion
    }
}
