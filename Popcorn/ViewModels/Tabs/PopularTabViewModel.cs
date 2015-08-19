using System;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using Popcorn.ViewModels.Main;

namespace Popcorn.ViewModels.Tabs
{
    /// <summary>
    /// The popular movies tab
    /// </summary>
    public sealed class PopularTabViewModel : TabsViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PopularTabViewModel class.
        /// </summary>
        public PopularTabViewModel()
        {
            RegisterMessages();
            RegisterCommands();
            TabName = LocalizationProviderHelper.GetLocalizedValue<string>("PopularTitleTab");
        }

        #endregion

        #region Methods

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<ChangeLanguageMessage>(
                this,
                language => { TabName = LocalizationProviderHelper.GetLocalizedValue<string>("PopularTitleTab"); });
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
                var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                mainViewModel.IsConnectionInError = false;
                await LoadMoviesAsync();
            });
        }

        #endregion

        #region Method -> LoadMoviesAsync

        /// <summary>
        /// Load next page
        /// </summary>
        public override async Task LoadMoviesAsync()
        {
            if (Page == LastPage)
                return;

            Page++;
            IsLoadingMovies = true;
            try
            {
                var movieResults =
                    await MovieService.GetPopularMoviesAsync(Page,
                        MaxMoviesPerPage,
                        CancellationLoadNextPageToken.Token);
                var movies = movieResults.Item1.ToList();
                MaxNumberOfMovies = movieResults.Item2;

                foreach (var movie in movies)
                {
                    Movies.Add(movie);
                }

                if (!movies.Any())
                    LastPage = Page;

                IsLoadingMovies = false;

                await MovieHistoryService.ComputeMovieHistoryAsync(movies);
                await MovieService.DownloadCoverImageAsync(movies);
            }
            catch (Exception)
            {
                Page--;
            }
            finally
            {
                IsLoadingMovies = false;
                IsMovieFound = Movies.Any();
                CurrentNumberOfMovies = Movies.Count();
                if (!IsMovieFound)
                    Page = 0;
            }
        }

        #endregion

        #endregion
    }
}