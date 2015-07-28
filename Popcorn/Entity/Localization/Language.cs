using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Popcorn.Entity.Localization
{
    /// <summary>
    /// Represents language as an entity in the database
    /// </summary>
    public class Language
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string LocalizedName { get; set; }
        public string EnglishName { get; set; }
        public string Culture { get; set; }
        public bool IsCurrentLanguage { get; set; }
    }
}
