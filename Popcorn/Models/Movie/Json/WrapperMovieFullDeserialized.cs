using Newtonsoft.Json;

namespace Popcorn.Models.Movie.Json
{
    public class WrapperMovieFullDeserialized
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("data")]
        public MovieFullDeserialized Movie { get; set; }

        [JsonProperty("meta")]
        public MetaDeserialized Meta { get; set; }
    }
}
