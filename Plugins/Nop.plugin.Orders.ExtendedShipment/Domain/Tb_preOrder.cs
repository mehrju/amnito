using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tb_preOrder : BaseEntity
    {
        public string PreOrderJson { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public int? OrderId { get; set; }
        public string UniqRefrenceId { get; set; }
    }
}
