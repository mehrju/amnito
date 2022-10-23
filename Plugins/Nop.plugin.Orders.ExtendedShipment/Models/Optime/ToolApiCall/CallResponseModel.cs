using Newtonsoft.Json;
using System;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall
{
    public partial class CallResponseModel
    {
        [JsonProperty("responseDateTime")]
        public DateTime ResponseDateTime { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }

}


