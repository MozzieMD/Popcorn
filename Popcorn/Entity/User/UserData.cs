using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Popcorn.Entity.User
{
    /// <summary>
    /// Represents user's data as an entity in the database
    /// </summary>
    public class UserData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<MovieHistory> MovieHistory { get; set; }
    }
}
