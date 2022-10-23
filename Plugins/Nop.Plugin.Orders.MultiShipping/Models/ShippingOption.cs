using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Orders.MultiShipping.Models
{

    public class MultiShipmentShippingOptions {
        public int ShipmentNumber { get; set; }
        public int OrginalShipmentNumber { get; set; }
        public List<int> ShoppingCartIds { get; set; }
        public int? ShippingAddressId { get; set; }
        public string DeliveryDate { get; set; }
        public ShippingOption SelectedShippingOption { get; set; }
    }
}
