using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Popcorn.Model.Account.Json
{
    public class UserDeserialized
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fullName")]
        public string Fullname { get; set; }

        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("emailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("joinDate")]
        public DateTime Joindate { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("claims")]
        public List<string> Claims { get; set; }
    }
}
