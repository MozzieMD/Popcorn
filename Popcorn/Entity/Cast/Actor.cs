using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Popcorn.Entity.Cast
{
    public class Actor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string CharacterName { get; set; }
        public string SmallImage { get; set; }
        public string MediumImage { get; set; }
        public string SmallImagePath { get; set; }
    }
}
