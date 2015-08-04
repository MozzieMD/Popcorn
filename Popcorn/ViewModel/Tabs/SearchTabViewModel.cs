using System;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.Comparers;
using System.Collections.Generic;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

namespace Popcorn.ViewModel.Tabs
{
    /// <summary>
    /// The search movies tab
    /// </summary>
    public class SearchTabViewModel : TabsViewModel
    {
        #region Property -> CancellationSearchMoviesToken

        /// <summary>
        /// Token to cancel searching movies
        /// </summary>
        protected CancellationTokenSource CancellationSearchMoviesToken { get; set; }
        #endregion

        #region Property -> LastPageFilterMapping

        /// <summary>
        /// Used to determine the last page of the searched criteria
        /// </summary>
        private Dictionary<string, int> LastPageFilterMapping { get; set; }

        #endregion

        #region Property -> SearchFilter

        /// <summary>
        /// The filter for searching movies
        /// </summary>
        public string SearchFilter { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// SearchTabViewModel
        /// </summary>
        public SearchTabViewModel()
        {
            CancellationSearchMoviesToken = new CancellationTokenSource();

            TabName = LocalizationProviderHelper.GetLocalizedValue<string>("SearchTitleTab");

            LastPageFilterMapping = new Dictionary<string, int>();

            Messenger.Default.Register<ChangeLanguageMessage>(
            this,
            language =>
            {
                TabName = LocalizationProviderHelper.GetLocalizedValue<string>("SearchTitleTab");
            });

            // Reload movies
            ReloadMovies = new RelayCommand(async () =>
            {
                await SearchMoviesAsync(SearchFilter);
                Messenger.Default.Send(new ConnectionErrorMessage(string.Empty, true));
            });
        }
        #endregion

        #region Methods

        #region Method -> SearchMoviesAsync

        /// <summary>
        /// Search movies
        /// </summary>
        /// <param name="searchFilter">The parameter of the search</param>
        public async Task SearchMoviesAsync(string searchFilter)
        {
            if (SearchFilter != searchFilter)
            {
                // We start an other search
                StopSearchingMovies();
                Movies.Clear();
                Page = 0;
            }

            SearchFilter = searchFilter;

            Page++;
            var lastPage = int.MaxValue;
            if (!LastPageFilterMapping.ContainsKey(searchFilter) || (LastPageFilterMapping.TryGetValue(searchFilter, out lastPage) && Page < lastPage))
            {
                try
                {
                    IsLoadingMovies = true;

                    var movieResults =
                        await ApiService.SearchMoviesAsync(searchFilter,
                        Page,
                        MaxMoviesPerPage,
                        CancellationSearchMoviesToken.Token);
                    var movies = movieResults.ToList();

                    // Download the cover image for each movie
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

                    if (!LastPageFilterMapping.ContainsKey(searchFilter) && !movies.Any())
                    {
                        LastPageFilterMapping.Add(searchFilter, Page);
                    }

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
        }

        #endregion

        #region Method -> StopSearchingMovies

        /// <summary>
        /// Cancel searching movies
        /// </summary>
        protected void StopSearchingMovies()
        {
            CancellationSearchMoviesToken?.Cancel(true);
            CancellationSearchMoviesToken?.Dispose();
            CancellationSearchMoviesToken = new CancellationTokenSource();
        }

        #endregion

        #endregion

        public override void Cleanup()
        {
            StopSearchingMovies();

            Messenger.Default.Unregister<ChangeLanguageMessage>(this);
            base.Cleanup();
        }
    }
}
