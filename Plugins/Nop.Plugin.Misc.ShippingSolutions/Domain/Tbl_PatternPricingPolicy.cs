using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
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
    public class Tbl_PatternPricingPolicy : BaseEntity
    {
        public Tbl_PatternPricingPolicy()
        {
            ListCategory = new List<SelectListItem>();
        }

        [Required]
        [ScaffoldColumn(false)]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyListCategory")]
        public int CategoryId { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyNamePattern")]
        [DataType(DataType.Text)]
        public String Name { get; set; }


        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMinCount")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MinCount { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMaxCount")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxCount { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMinWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MinWeight { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMaxWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxWeight { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyPercent")]
        [Range(0, float.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public float Percent { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMablagh")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public double Mablagh { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyTashim")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public double Tashim { get; set; }



        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyPercentTashim")]
        [Range(0, float.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public float PercentTashim { get; set; }



        [ScaffoldColumn(false)]
        public int IdParent { get; set; }


        [ScaffoldColumn(false)]
        public bool IsActive{ get; set; }

       

        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }


        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyListCategory")]
        public IList<SelectListItem> ListCategory { get; set; }


        
    }
}
