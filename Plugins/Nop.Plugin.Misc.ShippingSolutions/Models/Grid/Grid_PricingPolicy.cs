using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_PricingPolicy
    {
        public int Id { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_NameDealer")]
        //public string Grid_PricingPolicy_NameDealer { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_NameCustomer")]
        //public string Grid_PricingPolicy_NameCustomer { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_NameProvider")]
        //public string Grid_PricingPolicy_NameProvider { get; set; }                        
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_MinWeight")]
        public int Grid_PricingPolicy_MinWeight { get; set; }                           
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_MaxWeight")]
        public int Grid_PricingPolicy_MaxWeight { get; set; }                           
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_Percent")]
        public float Grid_PricingPolicy_Percent { get; set; }                             
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_Mablagh")]
        public double Grid_PricingPolicy_Mablagh { get; set; }                            
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_Tashim")]
        public double Grid_PricingPolicy_Tashim { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_PercentTashim")]
        public float Grid_PricingPolicy_PercentTashim { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_NameCountry")]
        //public string Grid_PricingPolicy_NameCountry { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_IsActive")]
        public bool Grid_PricingPolicy_IsActive { get; set; }                
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_DateInsert")]
        public DateTime Grid_PricingPolicy_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_DateUpdate")]
        public DateTime? Grid_PricingPolicy_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_UserInsert")]
        public string Grid_PricingPolicy_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_UserUpdate")]
        public string Grid_PricingPolicy_UserUpdate { get; set; }
    }
}
