using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain.PhoneOrder
{
    public class Tbl_PhoneOrder_Order : BaseEntity
    {
        public int PhoneOrderId { get; set; }
        public int OrderId { get; set; }
    }
}
