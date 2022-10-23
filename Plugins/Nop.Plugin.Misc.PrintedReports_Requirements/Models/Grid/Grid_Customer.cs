using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Customers;
using Nop.Web.Framework.Mvc.Models;
using System;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models.Grid
{
    [Validator(typeof(CustomerValidator))]
    public class Grid_Customer //: BaseNopEntityModel
    {


        public int Id { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FullName")]
        public string FullName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Username")]
        public string Username { get; set; }


        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.GridAvatarUrl")]
        public string AvatarUrl { get; set; }

        //customer roles
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.StateVerify")]
        public string StateVerify { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.DateVerify")]
        public Nullable<DateTime> DateVerify { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.NameUserVerify")]
        public string NameUserVerify { get; set; }

    }
}
