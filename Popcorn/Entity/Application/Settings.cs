using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Popcorn.Entity.Localization;

namespace Popcorn.Entity.Application
{
    /// <summary>
    /// Represents application's settings as an entity in the database
    /// </summary>
    public class Settings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }
        public virtual ICollection<Language> Languages { get; set; }
    }
}