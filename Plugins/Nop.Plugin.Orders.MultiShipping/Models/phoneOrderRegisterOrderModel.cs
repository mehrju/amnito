using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class phoneOrderRegisterOrderModel : BaseEntity
    {
        public Address SenderAddress { get; set; }
        public Tbl_Address_LatLong GeoPoint { get; set; }
        public int PhoneOrderId { get; set; }
        public int ServiceId { get; set; }
        public string description { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
    }
}
