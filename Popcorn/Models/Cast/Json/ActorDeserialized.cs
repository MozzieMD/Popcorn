using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Popcorn.Models.Cast.Json
{
    public class ActorDeserialized : ObservableObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("character_name")]
        public string CharacterName { get; set; }

        [JsonProperty("small_image")]
        public string SmallImage { get; set; }

        [JsonProperty("medium_image")]
        public string MediumImage { get; set; }
    }
}
