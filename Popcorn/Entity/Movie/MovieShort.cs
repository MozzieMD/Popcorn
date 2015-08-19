using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Popcorn.Entity.Movie
{
    /// <summary>
    /// Represents movie as an entity in the database
    /// </summary>
    public class MovieShort
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public int MovieId { get; set; }
        public bool IsFavorite { get; set; }
        public bool HasBeenSeen { get; set; }
        public string Url { get; set; }
        public string ImdbCode { get; set; }
        public string Title { get; set; }
        public string TitleLong { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }
        public int Runtime { get; set; }
        public virtual ICollection<Genre> Genres { get; set; }
        public string Language { get; set; }
        public string MpaRating { get; set; }
        public string SmallCoverImage { get; set; }
        public string MediumCoverImage { get; set; }
        public string State { get; set; }
        public virtual ICollection<Torrent> Torrents { get; set; }
        public string DateUploaded { get; set; }
        public int DateUploadedUnix { get; set; }
        public int ServerTime { get; set; }
        public string ServerTimezone { get; set; }
        public int ApiVersion { get; set; }
        public string ExecutionTime { get; set; }
        public string CoverImagePath { get; set; }
    }
}