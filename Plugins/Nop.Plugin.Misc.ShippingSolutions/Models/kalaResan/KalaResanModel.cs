using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.kalaResan
{
    public class RegisterParcelInputModel
    {
        public string apiCode { get; set; }
        public string postexShipmentCode { get; set; }
        public string senderName { get; set; }
        public string senderPhone { get; set; }
        public string senderAddr { get; set; }
        public string receiverName { get; set; }
        public string receiverPhone { get; set; }
        public string receiverAddr { get; set; }
        public int originCity { get; set; }
        public int destinationCity { get; set; }
        public List<PacketsDetail> packetsDetail { get; set; }
    }
    public class PacketsDetail
    {
        public int size { get; set; }
        public int count { get; set; }
        public string desc { get; set; }
    }
    public class RegisterParcelOutoutModel
    {
        public int error_code { get; set; }
        public string message { get; set; }
        public string shipment_id { get; set; }
        public string shipment_code { get; set; }
        public string final_price { get; set; }
        public string print_address { get; set; }
    }
    public class TrackingInputModel
    {
        public string apiCode { get; set; }
        public string shipmentCode { get; set; }
    }
    public class TrackingOutputtModel
    {
        public string shipmentCode { get; set; }
        public string status { get; set; }
    }
    public class CancelOrderOutputModel
    {
        public string message { get;set; }
        public string shipment_code { get;set; }
        public string status { get;set; }
    }
    public class GetPriceInputModel
    {
        public string apiCode { get; set;}
        public int originCity { get; set;}
        public int destinationCity { get; set;}
        public List<GetPricepacketsDetail> packetsDetail { get; set; }
    }
    public class GetPricepacketsDetail
    {
        public int size { get; set; }
        public int count { get; set; }
    }
    public class GetPriceOutputModel
    {
        public int shipment_cost { get; set; }
        public string error_code { get; set; }
        public string message { get; set; }
    }
}
