using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_DiscountPlan_AgentCustomer : BaseEntity
    {
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_Name")]
        [Required(ErrorMessage ="نام طرح اجباری میباشد")]
        [DataType(DataType.Text)]
        public String Name { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_OfAmount")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid double Number")]
        public double OfAmount { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_UpAmount")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid double Number")]
        public double UpAmount { get; set; }


        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_IsAgentt")]
        public bool IsAgent { get; set; }



        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_Percent")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid double Number")]
        public float _Percent { get; set; }


        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_ExpireDay")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid double Number")]
        public int ExpireDay { get; set; }


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
    }
}
