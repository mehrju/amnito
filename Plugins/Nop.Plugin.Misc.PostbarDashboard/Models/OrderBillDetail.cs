using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class OrderBillDetail
    {
        public string Username { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string RevicerPhoneNumber { get; set; }
        public int? OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string BoxContecnt { get; set; }
        public string ServiceName { get; set; }
        public string ServiceProviderName { get; set; }
        public string OrderDate { get; set; }
        public string SenderCountryName { get; set; }
        public string SenderCityName { get; set; }
        public string ReciverCountryName { get; set; }
        public string ReciverStateName { get; set; }
        public int? PostBasePrice { get; set; }
        public int? EngPrice { get; set; }
        public int? OrderDiscount { get; set; }
        public string GoodsCodPrice { get; set; }
        public int? SmsPrice { get; set; }
        public int? PrintLogoPrice { get; set; }
        public int? CartonCost { get; set; }
        public int? PackingPrice { get; set; }
        public int? AccessPrinterPrice { get; set; }
        public int? CompulsoryInsurancePrice { get; set; }
        public int? InsurancePrice { get; set; }
        public int? DepotCost { get; set; }
        public int? CollectPrice { get; set; }
        public string RegisterCost { get; set; }
        public int? OrderTotal { get; set; }
        public string paymentMethod { get; set; }
        public string SenderName { get; set; }
        public string ReceaiverName { get; set; }
    }
}
