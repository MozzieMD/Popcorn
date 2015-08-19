using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Popcorn.Entity.Movie
{
    public class MovieHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public virtual ICollection<MovieShort> MoviesShort { get; set; }
        public virtual ICollection<MovieFull> MoviesFull { get; set; }
    }
}
