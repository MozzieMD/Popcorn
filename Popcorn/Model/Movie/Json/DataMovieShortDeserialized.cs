using Newtonsoft.Json;
using System.Collections.Generic;

namespace Popcorn.Model.Movie.Json
{
    public class DataMovieShortDeserialized 
    {
        [JsonProperty("movie_count")]
        public int MovieCount { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("page_nombre")]
        public int PageNumber { get; set; }

        [JsonProperty("movies")]
        public IEnumerable<MovieShortDeserialized> Movies { get; set; }
    }
}
