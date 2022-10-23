using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Optime.ExecuteTask
{
    public class ExecuteTaskRequestModel
    {
        [JsonProperty("deliveryTaskId")]
        public int DeliveryTaskId { get; set; }
    }
}
