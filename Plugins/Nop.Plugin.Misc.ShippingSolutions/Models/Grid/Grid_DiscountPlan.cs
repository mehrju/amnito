using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_DiscountPlan
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_Name")]
        public string Grid_DiscountPlan_Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_OfAmount")]
        public double Grid_DiscountPlan_OfAmount { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_UpAmount")]
        public double Grid_DiscountPlan_UpAmount { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_IsAgent")]
        public bool Grid_DiscountPlan_IsAgent { get; set; }



        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_ExpireDay")]
        public int Grid_DiscountPlan_ExpireDay { get; set; }



        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_Percent")]
        public float Grid_DiscountPlan_Percent { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_IsActive")]
        public bool Grid_DiscountPlan_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_DateInsert")]
        public DateTime Grid_DiscountPlan_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_DateUpdate")]
        public DateTime? Grid_DiscountPlan_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_UserInsert")]
        public string Grid_DiscountPlan_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_UserUpdate")]
        public string Grid_DiscountPlan_UserUpdate { get; set; }
    }
}
