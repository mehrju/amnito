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
    public class Tbl_Dealer_Customer_ServiceProvider : BaseEntity
    {
        public Tbl_Dealer_Customer_ServiceProvider()
        {
            ListCustomer = new List<SelectListItem>();
            _ListProvider = new List<SelectListItem>();
            ListStateApplyPricingPolicy=new List<SelectListItem>();
        }

        // 1 Customer   2 Dealer 
        [ScaffoldColumn(false)]
        public int TypeUser { get; set; }

        [ScaffoldColumn(false)]
        public int CustomerId { get; set; }

        [ScaffoldColumn(false)]
        public int DealerId { get; set; }

        [ScaffoldColumn(false)]
        public int ProviderId { get; set; }


        [ScaffoldColumn(false)]
        public int StateApplyPricingPolicy { get; set; }



        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DealerCustomerStateMonth_Day")]
        public bool StateMonth_Day   { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DealerCustomerMaxCountpackage")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxCountpackage { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DealerCustomerMaxWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxWeight { get; set; }



        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.StateNegative_credit_amount")]
        public bool StateNegative_credit_amount { get; set; }


        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Negative_credit_amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public double Negative_credit_amount { get; set; }

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


        public Tbl_ServicesProviders Provider { get; set; }

        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DealerCustomerListCustomer")]
        public IList<SelectListItem> ListCustomer { get; set; }


        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DealerCustomer_ProviderId")]
        public IList<int> _ProviderId { get; set; }
        [NotMapped]
        public IList<SelectListItem> _ListProvider { get; set; }

        [NotMapped]
        public string NameCustomer { get; set; }

        //public List<Tbl_PricingPolicy> PricingPolicyCustomerDealer { get; set; }

        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.ListStateApplyPricingPolicy")]
        public IList<SelectListItem> ListStateApplyPricingPolicy { get; set; }
    }
}
