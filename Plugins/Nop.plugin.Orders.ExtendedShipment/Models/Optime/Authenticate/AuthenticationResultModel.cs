using Newtonsoft.Json;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.Authenticate
{
    public class AuthenticationResultModel
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }
    }
}
