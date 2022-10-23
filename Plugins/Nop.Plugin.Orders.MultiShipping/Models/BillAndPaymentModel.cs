using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class BillAndPaymentModel
    {
        public List<FactorItem> FactorItems { get; set; }
        public CheckoutPaymentMethodModel PaymentMethods { get; set; }
        public bool SafeBuy { get; set; }
    }
    public class OrderBillAndPaymentModel
    {
        public Order order { get; set; }
        public CheckoutPaymentMethodModel PaymentMethods { get; set; }
    }
    public class FactorItem
    {
        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
        public int OrderId { get; set; }
        public int ShipmentId { get; set; }
        public string TrackingNumber { get; set; }
        public string Base64TrackingNumber { get; set; }
        public List<BillItems> PostItems { get; set; }
        public List<BillItems> EngItems { get; set; }
        public int OrderTotal { get; set; }
        public int discountbyRule { get; set; }
        public decimal Weight { get; set; }
        public string GoodsType { get; set; }
        public bool IsCod { get; set; }
        public bool IsForeign { get; set; }
        public int CodGoodsPrice { get; set; }
        public DateTime CreateDate { get; set; }
        public bool hasPostPriceTax { get; set; }
    }
    public class BillItems
    {
        public int RowNumber { get; set; }
        public string description { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
    }
    public class PaymentMethod
    {
        public string PaymentMethodName { get; set; }
        public string SystemName { get; set; }
        public string ImageUrl { get; set; }
    }
}
