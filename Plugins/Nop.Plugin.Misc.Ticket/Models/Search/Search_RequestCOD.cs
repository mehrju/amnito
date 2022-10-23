using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_RequestCOD
    {
        public Search_RequestCOD()
        {
            SearchCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();
            ListStatus = new List<SelectListItem>();
            ListStores = new List<SelectListItem>();
            ListUsers = new List<SelectListItem>();
        }
        public bool ActiveSearch { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        public IList<int> SearchCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchUsername_customer")]
        public string SearchUsername_customer { get; set; }
        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchFirstName")]
        public string SearchFirstName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchLastName")]
        public string SearchLastName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchIdStatusRequestCOD")]
        public int SearchIdStatusRequestCOD { get; set; }
        public List<SelectListItem> ListStatus { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchStoreId")]
        public int SearchStoreId { get; set; }
        public List<SelectListItem> ListStores { get; set; }

        public List<SelectListItem> ListUsers { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchRequestCODNumber")]
        public int SearchRequestCODNumber { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchUsername_staff")]
        public int SearchUsername_Staff { get; set; }


        public int SearchId { get; set; }

    }
}
