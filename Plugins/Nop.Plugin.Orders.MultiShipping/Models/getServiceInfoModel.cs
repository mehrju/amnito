using Nop.plugin.Orders.ExtendedShipment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class getServiceInfoModel
    {
        public int senderCountry { get; set; }
        public int senderState { get; set; }
        public int receiverCountry { get; set; }
        public int receiverState { get; set; }
        public int weightItem { get; set; }
        public int AproximateValue { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int length { get; set; }
        public string dispatch_date { get; set; }
        public string UbbraTruckType { get; set; }
        public string VechileOptions { get; set; }
        public string UbbarPackingLoad { get; set; }
        public string Content { get; set; }
        public int receiver_ForeginCountry { get; set; }
        public string receiver_ForeginCountryNameEn { get; set; }
        public byte boxType { get; set; }
        public bool? IsCod{ get; set; }
        public bool IsFromAp{ get; set; }
        public bool IsFromSep { get; set; }
        public bool ShowPrivatePost { get; set; }
        public bool ShowDistributer { get; set; }
        public CustomAddressModel SenderAddress { get; set; }
        public CustomAddressModel ReciverAddress { get; set; }
        
    }
}
