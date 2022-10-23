using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_IncentivePlanCustomer : BaseEntity
    {
        public int CustomerId { get; set; }
        public bool IsActiveIncentivePlan { get; set; }
        public bool IsAutomaticIncentivePlan { get; set; }
        public int IdDiscountPlan { get; set; }
        public DateTime DateInsert { get; set; }
        public int IdUserInsert { get; set; }



    }
}
