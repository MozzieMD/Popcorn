using Newtonsoft.Json;

namespace Popcorn.Models.Movie.Json
{
    public class WrapperMovieShortDeserialized
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("data")]
        public DataMovieShortDeserialized Data { get; set; }

        [JsonProperty("@meta")]
        public MetaDeserialized Meta { get; set; }
    }
}
