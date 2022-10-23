using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class XtnShipmentModel: ShipmentModel
    {
        public string Address { get; set; }

    }
}
