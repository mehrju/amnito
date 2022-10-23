using Newtonsoft.Json;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.GetToolResponseCluster
{
    public class ResponseDataModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("vehicle")]
        public string Vehicle { get; set; }

        [JsonProperty("zone")]
        public int Zone { get; set; }

        [JsonProperty("plans")]
        public PlanModel Plans { get; set; }
    }
}