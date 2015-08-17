using Newtonsoft.Json;

namespace Popcorn.Models.Movie.Json
{
    public class MetaDeserialized
    {
        [JsonProperty("server_time")]
        public int server_time { get; set; }

        [JsonProperty("server_timezone")]
        public string server_timezone { get; set; }

        [JsonProperty("api_version")]
        public int api_version { get; set; }

        [JsonProperty("execution_time")]
        public string execution_time { get; set; }
    }
}
