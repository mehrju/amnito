using Newtonsoft.Json;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall
{
    public partial class FileContent
    {
        //id delkhah dar systeme postex
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("Vehicle")]
        public long Vehicle { get; set; } = 0;

        [JsonProperty("Latitude")]
        public string Latitude { get; set; } = null;

        [JsonProperty("Longitude")]
        public string Longitude { get; set; } = null;

        [JsonProperty("District")]
        public string District { get; set; } = "";

        [JsonProperty("MissionType")]
        public string MissionType { get; set; } = "p";

        [JsonProperty("ServiceTime")]
        public long ServiceTime { get; set; } = 5;

        [JsonProperty("Weight")]
        public double Weight { get; set; } = 1;

        [JsonProperty("Volume")]
        public double Volume { get; set; } = 1;

        [JsonProperty("CustomerName")]
        public string CustomerName { get; set; }

        [JsonProperty("CustomerPhoneNumber")]
        public string CustomerPhoneNumber { get; set; }

        [JsonProperty("CustomerTimeWindow")]
        public string CustomerTimeWindow { get; set; } = "9:00 – 21:00";
    }
}
