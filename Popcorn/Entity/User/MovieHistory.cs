using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Popcorn.Entity.User
{
    /// <summary>
    /// Represents movie's history as an entity in the database
    /// </summary>
    public class MovieHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string ImdbCode { get; set; }
        public bool Liked { get; set; }
        public bool Seen { get; set; }
    }
}
