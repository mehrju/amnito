using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Search
{
    public class Search_DiscountPlan
    {
        public bool Search_DiscountPlan_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_DiscountPlan_Name")]
        public string Search_DiscountPlan_Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_DiscountPlan_IsActive")]
        public bool Search_DiscountPlan_IsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_DiscountPlan_IsAgent")]
        public bool Search_DiscountPlan_IsAgent { get; set; }
    }
}
