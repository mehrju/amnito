using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_Product_PatternPricing : BaseEntity
    {
        public Tbl_Product_PatternPricing()
        {
            ListProduct = new List<SelectListItem>();
            ListStateApplyPricingPolicy = new List<SelectListItem>();
            ListPatternPricing = new List<SelectListItem>();

        }
        [ScaffoldColumn(false)]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingListProduct")]
        public int IdProduct { get; set; }
        [ScaffoldColumn(false)]
        public int IdPatternPricing { get; set; }
        [ScaffoldColumn(false)]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingListStateApplyPricingPolicy")]
        public int StateApplyPricingPolicy { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingListStateClaculateMonth")]
        public bool StateClaculateMonth { get; set; }

        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingPrice")]
        //[Range(0, double.MaxValue, ErrorMessage = "Please enter valid double Number")]
        //public double Price { get; set; }


        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }








        [NotMapped]
        public IList<SelectListItem> ListProduct { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListStateApplyPricingPolicy { get; set; }
        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingListPatternPricing")]
        public IList<int> _IdPatternPricing { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListPatternPricing { get; set; }
        [NotMapped]
        public string NameProduct { get; set; }
    }
}
