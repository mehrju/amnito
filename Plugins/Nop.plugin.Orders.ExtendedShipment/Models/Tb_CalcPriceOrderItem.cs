using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tb_CalcPriceOrderItem : BaseEntity
    {
        [ScaffoldColumn(false)]
        public int IncomePrice { get; set; }
        [ScaffoldColumn(false)]
        public int EngPrice { get; set; }
        [ScaffoldColumn(false)]
        public int AttrPrice { get; set; }
        [ScaffoldColumn(false)]
        public int OrderItemId { get; set; }
        [ScaffoldColumn(false)]
        public int? TashimValue { get; set; }
        [ScaffoldColumn(false)]
        public int? PricingPolicyId { get; set; }
    }
}
