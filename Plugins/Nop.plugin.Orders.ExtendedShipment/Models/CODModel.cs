using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CODModel
    {
        public int OrderId { get; set; }
        public string WebSiteUrl { get; set; }
        public string SupportPhoneNo { get; set; }
        public string DonnerBranch { get; set; }
        public string Country_State { get; set; }
        public byte[] BarcodeImage { get; set; }
        public string barcodeNo { get; set; }
        public int RowNum { get; set; }
        public string StoreName { get; set; }
        public string StorePhoneNo { get; set; }
        public string StoreUrl { get; set; }
        public string StoreAnswerTime { get; set; }
        public string StorePostCode { get; set; }
        public string StoreEmail { get; set; }
        public string StoreAddress { get; set; }
        public string E_NemadNo { get; set; }
        public int Weight { get; set; }
        public string Source { get; set; }
        public string PostTypeName { get; set; }
        public string PayeTypeName { get; set; }
        public string OrderDate { get; set; }
        public string ReadyConfirmDateTime { get; set; }
        public string ProductName { get; set; }
        public string ReciverFullName { get; set; }
        public string ReciverPhoneNo { get; set; }
        public string ReciverPostCode { get; set; }
        public string ReciverAddress { get; set; }
        public int ProductPrice { get; set; }
        public int PostPrice { get; set; }
        public int TaxValue { get; set; }
        public string DiscountValue { get; set; }
        public long Eng_Kala_Price { get; set; }
        public long TotalValue { get; set; }
        public string Destination { get; set; }
        public string sendToPostDate { get; set; }
        public int CodCost { get; set; }
        public int CodBmValue { get; set; }
        public string payType { get; set; }
        public bool isCod { get; set; }
        public int approximateValue { get; set; }
        public int ShipmentId { get; set; }
        public int AgentPercent { get; set; }
        public int CountryCode { get; set; }
        public bool IsSafeBuy { get; set; }

    }
    public class CODAttributesModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int ShipmentId { get; set; }
    }
}
