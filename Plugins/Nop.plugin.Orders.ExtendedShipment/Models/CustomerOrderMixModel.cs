using Nop.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CustomerOrderMixModel
    {
        public CustomerOrderListModel ColModel { get; set; }
        public ExtendedOrderListModel EolModel { get; set; }
    }
}
