using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.GetToolResponseCluster
{
    public partial class ResponseModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("responseDateTime")]
        public DateTime ResponseDateTime { get; set; }

        [JsonProperty("entity")]
        public Entity Entity { get; set; }
    }

    public partial class Entity
    {
        [JsonProperty("polygons")]
        public List<Polygon> Polygons { get; set; }

        [JsonProperty("deletedPins")]
        public List<Pin> DeletedPins { get; set; }

        [JsonProperty("notMets")]
        public List<Pin> NotMets { get; set; }

        [JsonProperty("remainPins")]
        public List<Pin> RemainPins { get; set; }

        [JsonProperty("planName")]
        public string PlanName { get; set; }

        [JsonProperty("planOwner")]
        public string PlanOwner { get; set; }

        [JsonProperty("planCreatedAt")]
        public DateTime PlanCreatedAt { get; set; }
    }

    public partial class Pin
    {
        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("packetId")]
        public string PacketId { get; set; }

        [JsonProperty("polygonId")]
        public string PolygonId { get; set; }

        [JsonProperty("orderId")]
        public List<string> OrderId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("recipientName")]
        public string RecipientName { get; set; }

        [JsonProperty("recipientPhoneNumber")]
        public string RecipientPhoneNumber { get; set; }

        [JsonProperty("shift")]
        public string Shift { get; set; }

        [JsonProperty("eta")]
        public string Eta { get; set; }

        [JsonProperty("meter")]
        public string Meter { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }

        [JsonProperty("missionType")]
        public string MissionType { get; set; }

        [JsonProperty("twNotMet")]
        public bool TwNotMet { get; set; }
    }

    public partial class Polygon
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("zone")]
        public long Zone { get; set; }

        [JsonProperty("subPlan")]
        public SubPlan SubPlan { get; set; }

        [JsonProperty("distance")]
        public long Distance { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("packetCount")]
        public long PacketCount { get; set; }

        [JsonProperty("pinCount")]
        public long PinCount { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("vehicleName")]
        public string VehicleName { get; set; }
    }

    public partial class SubPlan
    {
        [JsonProperty("pins")]
        public List<Pin> Pins { get; set; }
    }
}
