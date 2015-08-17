using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Popcorn.Services.Language;
using Popcorn.Services.Movie;
using Popcorn.Services.Settings;
using Popcorn.Services.User;
using Popcorn.ViewModels.Main;
using Popcorn.ViewModels.Movie;
using Popcorn.ViewModels.Search;
using Popcorn.ViewModels.Settings;

namespace Popcorn.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register(() => new ApplicationSettingsService());
            SimpleIoc.Default.Register(() => new LanguageService());
            SimpleIoc.Default.Register(() => new UserService());
            SimpleIoc.Default.Register(() => new MovieService());
            SimpleIoc.Default.Register(() => new MainViewModel());
            SimpleIoc.Default.Register(() => new SettingsViewModel());
            SimpleIoc.Default.Register(() => new MovieViewModel());
            SimpleIoc.Default.Register(() => new SearchViewModel());
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// Gets the Movie property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MovieViewModel MoviePage => ServiceLocator.Current.GetInstance<MovieViewModel>();

        /// <summary>
        /// Gets the Search property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SearchViewModel Search => ServiceLocator.Current.GetInstance<SearchViewModel>();

        /// <summary>
        /// Gets the Settings property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            if (SimpleIoc.Default.IsRegistered<MovieService>())
            {
                SimpleIoc.Default.Unregister<MovieService>();
            }

            if (SimpleIoc.Default.IsRegistered<ApplicationSettingsService>())
            {
                SimpleIoc.Default.Unregister<ApplicationSettingsService>();
            }

            if (SimpleIoc.Default.IsRegistered<UserService>())
            {
                SimpleIoc.Default.Unregister<UserService>();
            }

            if (SimpleIoc.Default.IsRegistered<LanguageService>())
            {
                SimpleIoc.Default.Unregister<LanguageService>();
            }

            if (SimpleIoc.Default.IsRegistered<MainViewModel>())
            {
                SimpleIoc.Default.Unregister<MainViewModel>();
            }

            if (SimpleIoc.Default.IsRegistered<SettingsViewModel>())
            {
                SimpleIoc.Default.Unregister<SettingsViewModel>();
            }

            if (SimpleIoc.Default.IsRegistered<MovieViewModel>())
            {
                SimpleIoc.Default.Unregister<MovieViewModel>();
            }

            if (SimpleIoc.Default.IsRegistered<SearchViewModel>())
            {
                SimpleIoc.Default.Unregister<SearchViewModel>();
            }
        }
    }
}