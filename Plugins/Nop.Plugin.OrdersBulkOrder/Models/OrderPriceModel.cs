using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.BulkOrder.Models
{
    public class OrderPriceModel
    {
        public int CartonPrice { get; set; }
        public string CartonName { get; set; }
        public List<ServicePrice> ServicePrices { get; set; }
        public int SmsPrice { get; set; }
        public int PrintPrice { get; set; }
        public int InsurancePrice { get; set; }
        public int LogoPrice { get; set; }
        public string SLA { get; set; }
    }

    public class OrderPriceSqlModel
    {
        public int AttrPrice { get; set; }
        public string ProductAttrName { get; set; }
        public string ProductAttrValue { get; set; }
    }


    public class ServicePrice
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Price { get; set; }
        public string PriceStr
        {
            get
            {
                return Price.ToString("N0") + " ریال";
            }
        }
        public int TotalPrice { get; set; }
        public string TotalPriceStr
        {
            get
            {
                return TotalPrice.ToString("N0") + " ریال";
            }
        }
        public int CollectionPrice { get; set; }
        public string SLA { get; set; }
        public int EngPrice { get; set; }
    }
}
