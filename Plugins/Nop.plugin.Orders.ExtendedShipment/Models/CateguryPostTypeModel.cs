using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CateguryPostTypeModel:BaseEntity
    {
        public int CateguryId { get; set; }
        public string CateguryName { get; set; }
        public int PostType { get; set; }
    }
}
