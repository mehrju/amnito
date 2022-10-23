using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models.Search
{
    public class Search_Customer
    {
        public Search_Customer()
        {
            SearchCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();
        }
        public bool ActiveSearch { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        public IList<int> SearchCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchUsername")]
        public string SearchUsername { get; set; }
        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchFirstName")]
        public string SearchFirstName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchLastName")]
        public string SearchLastName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.AllUser")]
        //public bool AllUser { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.UserNotOkAvatar")]
        public bool UserNotOkAvatar { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.UserOkAvatar")]
        public bool UserOkAvatar { get; set; }

        public int SearchId { get; set; }
    }
}
