using System.Collections.Generic;
using Newtonsoft.Json;

namespace Popcorn.Model.Subtitle.Json
{
    public class SubtitlesWrapperDeserialized
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("lastModified")]
        public int LastModified { get; set; }

        [JsonProperty("subtitles")]
        public int SubtitlesCount { get; set; }

        [JsonProperty("subs")]
        public Dictionary<string, Dictionary<string, List<SubtitleDeserialized>>> Subtitles{ get; set; }
    }
}
