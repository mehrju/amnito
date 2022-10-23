using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input
{
    public partial class Branch : BaseInput
    {

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonIgnore]
        public List<double[]> AreaArray { get; set; }

        [JsonProperty("area", NullValueHandling = NullValueHandling.Ignore)]
        public string Area
        {
            get
            {
                if (AreaArray == null)
                    return null;
                return $"[{string.Join(",", AreaArray.Select(p => $"[{string.Join(",", p.Reverse())}]"))}]";
            }
        }

        [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
        public string Phone { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("description",NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}
