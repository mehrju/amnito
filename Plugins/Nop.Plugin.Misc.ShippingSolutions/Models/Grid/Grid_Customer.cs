using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Customers;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    
    public class Grid_Customer //: BaseNopEntityModel
    {


        public int Id { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FullName")]
        public string FullName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Username")]
        public string Username { get; set; }


        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Email")]
        public string Email { get; set; }

        //customer roles
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.StateNegative_credit_amount")]
        public bool Grid_State_Negative_credit { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Negative_credit_amount")]
        public double Grid_Negative_credit_amount { get; set; }
    }
}
