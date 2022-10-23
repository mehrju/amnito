using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Search
{
   public class Search_PatternPricingPolicy
    {
        public Search_PatternPricingPolicy()
        {
            ListCategory = new List<SelectListItem>();
        }

        public bool ActiveSearch { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyListCategory")]
        public int SearchCategoryId { get; set; }

        public IList<SelectListItem> ListCategory { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsActive")]
        public bool SearchIsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyNamePattern")]
        public String SearchName { get; set; }


        
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMinCount")]
        public int SearchMinCount { get; set; }

        
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMaxCount")]
        public int SearchMaxCount { get; set; }
    }
}
