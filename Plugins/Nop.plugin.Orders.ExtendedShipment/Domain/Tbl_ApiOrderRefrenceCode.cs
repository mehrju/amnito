using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_ApiOrderRefrenceCode : BaseEntity
    {
        public int? OrderId { get; set; }
        public int CustomerId { get; set; }
        public string RefrenceCode { get; set; }
    }
}
