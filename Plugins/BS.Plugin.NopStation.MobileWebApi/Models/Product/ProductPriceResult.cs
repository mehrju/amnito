using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Product
{
    public class ProductPriceResult : BaseResult
    {
        public int CartoonPrice { get; set; }
        public int CollectionPrice { get; set; }
        public int SmsPrice { get; set; }
        //public int PrintPrice { get; set; }
        public int InsurancePrice { get; set; }
        public int LogoPrice { get; set; }
        public int BillPrintPrice { get; set; }
        public List<ServicePrice> ServicePrices { get; set; }
    }


    public class ServicePrice
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Price { get; set; }
        public int TotalPrice { get; set; }
        public int TotalPriceIncludeTax { get; set; }
        public string SLA { get; set; }
        public int CollectionPrice { get; set; }

    }
}
