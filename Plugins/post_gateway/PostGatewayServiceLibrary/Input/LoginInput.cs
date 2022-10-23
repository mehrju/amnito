using Newtonsoft.Json;

namespace FrotelServiceLibrary.Input
{
    public class LoginInput
    {
        [JsonProperty("usrpost")]
        public string UserName { get; set; } 

        [JsonProperty("pwdpost")]
        public string Password { get; set; }
    }
}
