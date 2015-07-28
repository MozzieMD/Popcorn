using System.Data.Entity;
using Popcorn.Entity.Settings;
using Popcorn.Entity.User;

namespace Popcorn.Entity
{
    /// <summary>
    /// Database context used in the whole application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public DbSet<UserData> UserData { get; set; }
    }
}
