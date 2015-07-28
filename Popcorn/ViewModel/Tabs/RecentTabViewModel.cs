using System;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Comparers;
using GalaSoft.MvvmLight.Command;

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
                Task.Run(async () => await LoadNextPageAsync());
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
            catch
            {
                IsLoadingMovies = false;

                if (!Movies.Any())
                {
                    IsMovieFound = false;
                }
                else
                {
                    IsMovieFound = true;
                }

                Page--;
            }
        }

        #endregion

        public override void Cleanup()
        {
            Messenger.Default.Unregister<ChangeLanguageMessage>(this);
            base.Cleanup();
        }
    }
}
