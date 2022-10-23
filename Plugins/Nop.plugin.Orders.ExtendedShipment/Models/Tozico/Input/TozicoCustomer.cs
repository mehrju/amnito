using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input
{
    public partial class TozicoCustomer : BaseInput
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("branch")]
        public long? Branch { get; set; }

        [JsonIgnore]
        public StateEnum StateEnum { get; set; }

        [JsonProperty("state")]
        public long State
        {
            get
            {
                return (long)StateEnum;
            }
        }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("requests")]
        public List<TozicoCustomerRequest> Requests { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
