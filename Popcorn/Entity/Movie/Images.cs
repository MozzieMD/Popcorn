using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Popcorn.Entity.Movie
{
    public class Images
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string BackgroundImage { get; set; }
        public string SmallCoverImage { get; set; }
        public string MediumCoverImage { get; set; }
        public string LargeCoverImage { get; set; }
        public string MediumScreenshotImage1 { get; set; }
        public string MediumScreenshotImage2 { get; set; }
        public string MediumScreenshotImage3 { get; set; }
        public string LargeScreenshotImage1 { get; set; }
        public string LargeScreenshotImage2 { get; set; }
        public string LargeScreenshotImage3 { get; set; }
    }
}
