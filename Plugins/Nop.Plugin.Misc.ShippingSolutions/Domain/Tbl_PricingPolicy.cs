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
    public class Tbl_PricingPolicy : BaseEntity
    {

        //   1 Customer   2 Dealer  3 Provider
        [ScaffoldColumn(false)]
        public int TypeUser { get; set; }

        [ScaffoldColumn(false)]
        public int ProviderId { get; set; }

        [ScaffoldColumn(false)]
        public int Dealer_Customer_Id { get; set; }

        [ScaffoldColumn(false)]
        public int CountryId { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyMinWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MinWeight { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyMaxWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxWeight { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyPercent")]
        [Range(0, float.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public float Percent { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyMablagh")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public double Mablagh { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyTashim")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public double Tashim { get; set; }


        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyPercentTashim")]
        [Range(0, float.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public float PercentTashim { get; set; }


        
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


        // public Tbl_ServicesProviders Provider { get; set; }
        // public Tbl_Dealer_Customer_ServiceProvider DealerCustomer { get; set; }
        // customer

        //[NotMapped]
        //public IList<SelectListItem> ListProvider { get; set; }
        //[NotMapped]
        //public IList<SelectListItem> ListDealer { get; set; }

        [NotMapped]
        public String NameUser { get; set; }
    }
}
