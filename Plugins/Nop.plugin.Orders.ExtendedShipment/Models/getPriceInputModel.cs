using Nop.plugin.Orders.ExtendedShipment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class getPriceInputModel
    {
        public int Weight { get; set; }
        public int ServiceId { get; set; }
        public int SenderStateId { get; set; }
        public int ReaciverStateId { get; set; }
        public string SenderStateName { get; set; }
        public string ReaciverStateName { get; set; }
        public int ReciverCountry { get; set; }
        public string ReciverCountryCode { get; set; }
        public int AproximateValue { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public byte ConsType { get; set; }
        public DateTime? dispatch_date { get; set; }
        public string UbbraTruckType { get; set; }
        public string VechileOptions { get; set; }
        public string UbbarPackingLoad { get; set; }
        public string Content { get; set; }
        public string CartonSizeName { get; set; }
        public string ErrorMessage { get; set; }
        public int ApTypeName { get; set; }
        public CustomAddressModel SenderAddress { get; set; }
        public CustomAddressModel ReciverAddress { get; set; }
        public int BlueSky_ParcelType { get; set; }
        public bool IsExtraForpostexPlus { get; set; }
        public int count { get; set; }
        public int? OrderItemId { get; set; }
    }
}
