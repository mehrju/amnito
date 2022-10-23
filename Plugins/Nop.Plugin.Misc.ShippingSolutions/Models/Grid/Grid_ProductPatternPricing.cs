using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_ProductPatternPricing
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_ProductName")]
        public string Grid_ProductPatternPricing_ProductName { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_StateApplyPricingPolicy")]
        public string Grid_ProductPatternPricing_StateApplyPricingPolicy { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_StateClaculateMonth")]
        public bool Grid_ProductPatternPricing_StateClaculateMonth { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_PatternNames")]
        public string Grid_ProductPatternPricing_PatternNames { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_IsActive")]
        public bool Grid_ProductPatternPricing_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_DateInsert")]
        public DateTime Grid_ProductPatternPricing_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_DateUpdate")]
        public DateTime? Grid_ProductPatternPricing_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_UserInsert")]
        public string Grid_ProductPatternPricing_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_UserUpdate")]
        public string Grid_ProductPatternPricing_UserUpdate { get; set; }
    }
}
