using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }

        public string CategoryName { get; set; }

        public string OrderDate { get; set; }

        public string OrderStatus { get; set; }

        public long? OrderTotal { get; set; }

        public string PaymentStatus { get; set; }
        public int PaymentStatusId { get; set; }
        public int OrderStatusId { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
        public string strBillingAddress { get; set; }
        public string ReceaiverName { get; set; }
        public string SenderName { get; set; }
        public string ShipmentsState { get; set; }
    }
}
