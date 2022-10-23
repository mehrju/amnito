using Newtonsoft.Json;
using System.Collections.Generic;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.GetToolResponseCluster
{
    public class ResponseResultModel
    {
        // Dispatching and route optimization result
        [JsonProperty("data")]
        public List<ResponseDataModel> Data { get; set; }
    }
}
