using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class COD58mModel
    {
        public int orderId { get; set; }
        public byte[] Barcode { get; set; }
        public string BarcodeNo { get; set; }
        public int Weight { get; set; }
        public string OrderDate { get; set; }
        public string ReciverFullName { get; set; }
        public string ReciverPhoneNo { get; set; }
        public string ReciverPostCode { get; set; }
        public string ReciverAddress { get; set; }
        public string ReciverCountry { get; set; }
        public string ReciverState { get; set; }

        public string SenderFullName { get; set; }
        public string SenderPhoneNo { get; set; }
        public string SenderPostCode { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCountry { get; set; }
        public string SenderState { get; set; }
        public List<ProductAttribute> ProductAttrbiutes { get; set; }
        public long PostPrice { get; set; }
        public long PostTaxValue { get; set; }
        public long PostTotalValue { get; set; }
        public long EngPrice { get; set; }
        public long EngTaxValue { get; set; }
        public long EngTotalValue { get; set; }
        public long TotalValue { get; set; }
        public int orderItemId { get; set; }
        public string ProductName { get; set; }
        public string sendToPostDate { get; set; }
    }
    public class ProductAttribute
    {
        public int orderItemId { get; set; }
        public string name { get; set; }
        public long value { get; set; }
    }
}
