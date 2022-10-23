using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.GetToolResponseCluster
{
    public class PinModel
    {
        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("noGeo")]
        public bool NoGeo { get; set; }

        [JsonProperty("packet_id")]
        public string PacketId { get; set; }

        [JsonProperty("order_id")]
        public IList<string> OrderId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("vehicle")]
        public string Vehicle { get; set; }

        [JsonProperty("hasChildren")]
        public bool HasChildren { get; set; }

        [JsonProperty("recipientName")]
        public string RecipientName { get; set; }

        [JsonProperty("recipientPhoneNumber")]
        public string RecipientPhoneNumber { get; set; }

        [JsonProperty("color_id")]
        public string ColorId { get; set; }

        [JsonProperty("polygone_id")]
        public string PolygoneId { get; set; }

        [JsonProperty("shift")]
        public string Shift { get; set; }

        [JsonProperty("est")]
        public string Est { get; set; }

        [JsonProperty("children")]
        public IList<object> Children { get; set; }

        [JsonProperty("coords")]
        public IList<double> Coords { get; set; }
    }
}