using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_PatternPricingPolicy
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_Category_Name")]
        public string Grid_PatternPricingPolicy_Category_Name { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_IsActive")]
        public bool Grid_PatternPricingPolicy_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_DateInsert")]
        public DateTime Grid_PatternPricingPolicy_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_DateUpdate")]
        public DateTime? Grid_PatternPricingPolicy_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_UserInsert")]
        public string Grid_PatternPricingPolicy_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_UserUpdate")]
        public string Grid_PatternPricingPolicy_UserUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyNamePattern")]
        public String Grid_PatternPricingPolicy_Name { get; set; }



        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMinCount")]
        public int Grid_PatternPricingPolicy_MinCount { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMaxCount")]
        public int Grid_PatternPricingPolicy_MaxCount { get; set; }
    }
}
