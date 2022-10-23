using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Search
{
    public class Search_ProductPatternPricing
    {
        public Search_ProductPatternPricing()
        {
            ListPatternPricing = new List<SelectListItem>();
            ListStateApplyPricingPolicy = new List<SelectListItem>();

        }

        public bool Search_ProductPatternPricing_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_ProductPatternPricing_ProductName")]
        public string Search_ProductPatternPricing_ProductName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_ProductPatternPricing_IsActive")]
        public bool Search_ProductPatternPricing_IsActive { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_ProductPatternPricing_IdPatternPricing")]
        public int Search_ProductPatternPricing_IdPatternPricing { get; set; }
        public IList<SelectListItem> ListPatternPricing { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_ProductPatternPricing_IdStateApplyPricingPolicy")]
        public int Search_ProductPatternPricing_IdStateApplyPricingPolicy { get; set; }
        public IList<SelectListItem> ListStateApplyPricingPolicy { get; internal set; }
    }
}
