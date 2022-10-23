using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_OptimeListDetailes:BaseEntity
    {
        public int ListId { get; set; }
        public int ShipmentId { get; set; }
    }
}
