using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input
{
    public class TozicoCustomerRequest
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("weigth")]
        public long Weigth { get; set; }

        [JsonProperty("volume")]
        public long Volume { get; set; }

        [JsonProperty("volume_type")]
        public long VolumeType { get; set; }
    }
}
