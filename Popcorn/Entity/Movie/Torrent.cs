using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Popcorn.Entity.Movie
{
    public class Torrent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }
        public string Hash { get; set; }
        public string Quality { get; set; }
        public string Framerate { get; set; }
        public string Resolution { get; set; }
        public int Seeds { get; set; }
        public int Peers { get; set; }
        public string Size { get; set; }
        public long SizeBytes { get; set; }
        public string DateUploaded { get; set; }
        public int DateUploadedMix { get; set; }
    }
}
