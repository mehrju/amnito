using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrotelServiceLibrary.Output
{
    public class OrderPayOutput : BaseOutput
    {
        [JsonProperty("vaziat")]
        public string Status { get; set; }

        [JsonProperty("factor")]
        public string Factor { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("order_type")]
        public string OrderType { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("remainder_price")]
        public int RemainderPrice { get; set; }

        [JsonProperty("pay_price")]
        public int PayPrice { get; set; }

        [JsonProperty("banks")]
        public Bank[] Banks { get; set; }
    }
}
