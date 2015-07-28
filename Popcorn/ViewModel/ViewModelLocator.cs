using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Popcorn.Service.Api;
using Popcorn.Service.Language;
using Popcorn.Service.Settings;
using Popcorn.Service.User;
using Popcorn.ViewModel.Movie;
using Popcorn.ViewModel.Search;
using Popcorn.ViewModel.Settings;

namespace Popcorn.ViewModel
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
            SimpleIoc.Default.Register<IApplicationSettingsService, ApplicationSettingsService>();
            SimpleIoc.Default.Register<ILanguageService, LanguageService>();
            SimpleIoc.Default.Register<IUserDataService, UserDataService>();
            SimpleIoc.Default.Register<IApiService, ApiService>();
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
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Gets the Movie property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MovieViewModel MoviePage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MovieViewModel>();
            }
        }

        /// <summary>
        /// Gets the Search property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SearchViewModel Search
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SearchViewModel>();
            }
        }

        /// <summary>
        /// Gets the Settings property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            if (SimpleIoc.Default.IsRegistered<IApiService>())
            {
                SimpleIoc.Default.Unregister<IApiService>();
            }
            if (SimpleIoc.Default.IsRegistered<IApplicationSettingsService>())
            {
                SimpleIoc.Default.Unregister<IApplicationSettingsService>();
            }
            if (SimpleIoc.Default.IsRegistered<IUserDataService>())
            {
                SimpleIoc.Default.Unregister<IUserDataService>();
            }
            if (SimpleIoc.Default.IsRegistered<ILanguageService>())
            {
                SimpleIoc.Default.Unregister<ILanguageService>();
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