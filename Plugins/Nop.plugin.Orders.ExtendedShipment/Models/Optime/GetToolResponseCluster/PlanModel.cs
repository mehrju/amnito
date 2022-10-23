using Newtonsoft.Json;
using System.Collections.Generic;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.GetToolResponseCluster
{
    public class PlanModel
    {
        [JsonProperty("u_id")]
        public string UId { get; set; }

        [JsonProperty("driver")]
        public object Driver { get; set; }

        [JsonProperty("pins")]
        public IList<PinModel> Pins { get; set; }

        [JsonProperty("remain")]
        public IList<object> Remain { get; set; }

        [JsonProperty("deletedPins")]
        public IList<object> DeletedPins { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("isReOptimizing")]
        public bool IsReOptimizing { get; set; }
    }
}