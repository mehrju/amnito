using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Customers;
using Nop.Web.Framework.Mvc.Models;
using System;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
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

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.DepartmentTicketName")]
        public string DepartmentTicketName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_IsActive")]
        public bool Grid_Staff_IsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_DateInsert")]
        public DateTime Grid_DepartmentTicket_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_DateUpdate")]
        public DateTime? Grid_DepartmentTicket_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_UserInsert")]
        public string Grid_DepartmentTicket_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_UserUpdate")]
        public string Grid_DepartmentTicket_UserUpdate { get; set; }
    }
}
