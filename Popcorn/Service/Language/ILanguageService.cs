using System.Collections.Generic;
using System.Threading.Tasks;
using Popcorn.Model.Localization;

namespace Popcorn.Service.Language
{
    /// <summary>
    /// Used to describe a language service
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Get all available languages from the database
        /// </summary>
        /// <returns>All available languages</returns>
        Task<ICollection<ILanguage>> GetAvailableLanguagesAsync();

        /// <summary>
        /// Get the current language of the application
        /// </summary>
        /// <returns>Current language</returns>
        Task SetCurrentLanguageAsync(ILanguage language);

        /// <summary>
        /// Get the current language of the application
        /// </summary>
        Task<ILanguage> GetCurrentLanguageAsync();
    }
}
