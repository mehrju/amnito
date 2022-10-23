using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.PreOrderModel
{
    public class FavaWebhookModel
    {
        public int OrderId { get; set; }
        public string UniqueReferenceNo { get; set; }
        public string TrackingNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; } = "پرداخت شده";
    }
}
