using Newtonsoft.Json;

namespace Popcorn.Models.Movie.Json
{
    public class MetaDeserialized
    {
        [JsonProperty("server_time")]
        public int ServerTime { get; set; }

        [JsonProperty("server_timezone")]
        public string ServerTimezone { get; set; }

        [JsonProperty("api_version")]
        public int ApiVersion { get; set; }

        [JsonProperty("execution_time")]
        public string ExecutionTime { get; set; }
    }
}
