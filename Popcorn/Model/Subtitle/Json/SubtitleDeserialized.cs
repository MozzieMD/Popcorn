using System;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Popcorn.Model.Subtitle.Json
{
    public class SubtitleDeserialized : ObservableObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("hi")]
        public int Hi { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
