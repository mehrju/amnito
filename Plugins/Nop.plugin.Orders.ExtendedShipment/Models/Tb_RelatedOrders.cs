using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tb_RelatedOrders : BaseEntity
    {
        [ScaffoldColumn(false)]
        public int ParentOrderId { get; set; }
        [ScaffoldColumn(false)]
        public int ChildOrderId { get; set; } 
        [ScaffoldColumn(false)]
        public int ChildOrderPrice { get; set; }
    }
}
