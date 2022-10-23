using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.plugin.Orders.ExtendedShipment.JsonConvertors;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class OrderSearchCondition
    {
        public string OrderSerialFrom { get; set; }
        public string OrderSerialTo { get; set; }
        public int PayStatus { get; set; }
        public int OrderStatus { get; set; }
        public string RecieverName { get; set; }
        public int? RecieverProvinceId { get; set; }
        public int? RecieverCityId { get; set; }
        public int? SenderProvinceId { get; set; }
        public int? SenderCityId { get; set; }
        [JsonConverter(typeof(PersianToDateTimeConvertor))]
        public DateTime? FromDate { get; set; }
        [JsonConverter(typeof(PersianToDateTimeConvertor))]
        public DateTime? ToDate { get; set; }
        public int ServiceTypes { get; set; }
        public int CustomerId { get; set; }
        public int StoreId { get; set; }

    }
}
