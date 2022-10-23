using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_PatternPricingPolicy1
    {
        public int Id { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMinCount")]
        public int Grid_PricingPolicy_MinCount { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMaxCount")]
        public int Grid_PricingPolicy_MaxCount { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMinWeight")]
        public int Grid_PricingPolicy_MinWeight { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMaxWeight")]
        public int Grid_PricingPolicy_MaxWeight { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyPercent")]
        public float Grid_PricingPolicy_Percent { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMablagh")]
        public double Grid_PricingPolicy_Mablagh { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyTashim")]
        public double Grid_PricingPolicy_Tashim { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyPercentTashim")]
        public float Grid_PricingPolicy_PercentTashim { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_IsActive")]
        public bool Grid_PricingPolicy_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_DateInsert")]
        public DateTime Grid_PricingPolicy_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_DateUpdate")]
        public DateTime? Grid_PricingPolicy_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_UserInsert")]
        public string Grid_PricingPolicy_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_UserUpdate")]
        public string Grid_PricingPolicy_UserUpdate { get; set; }
    }
}
