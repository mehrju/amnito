using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class OrderTrackingBarcode
    {
        public int OrderId { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string SenderStateProvince { get; set; }
        public string SenderCity { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string ReceiverStateProvince { get; set; }
        public string ReceiverCity { get; set; }
        public string TrackingNumber { get; set; }
        public string CreateDatePersian { get; set; }
        public DateTime CreateDate { get; set; }
        public string ExactWeight { get; set; }
        public string ReceaiverName { get; set; }
        public string SenderName { get; set; }
        [NotMapped]
        public byte[] BarCodeImage { get; set; }
    }
    public class SearchedOrder
    {
        public int OrderId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string OrderDate { get; set; }
        public long OrderTotal { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentStatusId { get; set; }
        public string strBillingAddress { get; set; }
        public string ShipmentsState { get; set; }

    }
}
