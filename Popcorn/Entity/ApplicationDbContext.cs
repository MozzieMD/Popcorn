using System.Data.Entity;
using Popcorn.Entity.Application;
using Popcorn.Entity.Movie;

namespace Popcorn.Entity
{
    /// <summary>
    /// Database context used in the whole application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Settings> Settings { get; set; }
        public DbSet<MovieHistory> MovieHistory { get; set; }
    }
}