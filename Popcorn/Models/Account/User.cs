using Popcorn.Models.Account.Json;

namespace Popcorn.Models.Account
{
    public class User : UserDeserialized
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Bearer { get; set; }
    }
}
