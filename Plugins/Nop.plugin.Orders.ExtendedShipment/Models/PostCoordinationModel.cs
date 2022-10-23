using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class PostCoordinationModel : BaseEntity
    {
        public int orderId { get; set; }
        public DateTime? CoordinationDate { get; set; }
        public string Desc { get; set; }
    }
}
