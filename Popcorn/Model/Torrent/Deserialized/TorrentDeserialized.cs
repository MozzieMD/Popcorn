using Newtonsoft.Json;

namespace Popcorn.Model.Torrent
{
    public class TorrentDeserialized
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("framerate")]
        public string Framerate { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("seeds")]
        public int Seeds { get; set; }

        [JsonProperty("peers")]
        public int Peers { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("size_bytes")]
        public long SizeBytes { get; set; }

        [JsonProperty("date_uploaded")]
        public string DateUploaded { get; set; }

        [JsonProperty("date_uploaded_unix")]
        public int DateUploadedMix { get; set; }
    }
}
