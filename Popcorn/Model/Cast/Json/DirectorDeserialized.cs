using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Popcorn.Model.Cast.Json
{
    public class DirectorDeserialized : ObservableObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("small_image")]
        public string SmallImage { get; set; }

        [JsonProperty("medium_image")]
        public string MediumImage { get; set; }
    }
}
