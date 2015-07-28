using System.Threading.Tasks;

namespace Popcorn.Service.Settings
{
    /// <summary>
    /// Interface used to describe the application's settings service
    /// </summary>
    interface IApplicationSettingsService
    {
        /// <summary>
        /// Scaffold ApplicationSettings table on database if empty
        /// </summary>
        Task CreateApplicationSettingsAsync();
    }
}
