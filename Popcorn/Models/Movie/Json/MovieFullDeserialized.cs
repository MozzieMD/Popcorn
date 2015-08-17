using System.Collections.Generic;
using Newtonsoft.Json;
using Popcorn.Models.Torrent;
using GalaSoft.MvvmLight;
using Popcorn.Models.Cast;

namespace Popcorn.Models.Movie.Json
{
    public class MovieFullDeserialized : ObservableObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("imdb_code")]
        public string ImdbCode { get; set; }

        [JsonProperty("title")]
        protected string Title;

        [JsonProperty("title_long")]
        public string TitleLong { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("runtime")]
        public int Runtime { get; set; }

        [JsonProperty("genres")]
        protected IEnumerable<string> Genres;

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("mpa_rating")]
        public string MpaRating { get; set; }

        [JsonProperty("download_count")]
        public string DownloadCount { get; set; }

        [JsonProperty("like_count")]
        public string LikeCount { get; set; }

        [JsonProperty("rt_critics_score")]
        public string RtCrtiticsScore { get; set; }

        [JsonProperty("rt_critics_rating")]
        public string RtCriticsRating { get; set; }

        [JsonProperty("rt_audience_score")]
        public string RtAudienceScore { get; set; }

        [JsonProperty("rt_audience_rating")]
        public string RtAudienceRating { get; set; }

        [JsonProperty("description_intro")]
        public string DescriptionIntro { get; set; }

        [JsonProperty("description_full")]
        protected string DescriptionFull;

        [JsonProperty("yt_trailer_code")]
        public string YtTrailerCode { get; set; }

        [JsonProperty("images")]
        public MovieImagesDeserialized Images { get; set; }

        [JsonProperty("directors")]
        public IEnumerable<Director> Directors { get; set; }

        [JsonProperty("actors")]
        public IEnumerable<Actor> Actors { get; set; }

        [JsonProperty("torrents")]
        public IEnumerable<TorrentDeserialized> Torrents { get; set; }

        [JsonProperty("date_uploaded")]
        public string DateUploaded { get; set; }

        [JsonProperty("date_uploaded_unix")]
        public int DateUploadedUnix { get; set; }
    }
}
