using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_Ticket
    {
        public Search_Ticket()
        {
            SearchCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();
            ListDepartmentTicket = new List<SelectListItem>();
            ListStatus = new List<SelectListItem>();
            ListPriority = new List<SelectListItem>();
            ListStores = new List<SelectListItem>();
            ListUsers = new List<SelectListItem>();
            ListcategoryTicket = new List<SelectListItem>();
            ListOstanTicket = new List<SelectListItem>();
        }
        public bool ActiveSearch { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_Damages_IsDeleted")]
        public bool Search_Ticket_Isdeleted { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        public IList<int> SearchCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchUsername_customer")]
        public int SearchUsername_customer { get; set; }
        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchFirstName")]
        public string SearchFirstName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchLastName")]
        public string SearchLastName { get; set; }




        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchDepartmentId")]
        public int SearchDepartmentId { get; set; }
        public List<SelectListItem> ListDepartmentTicket { get; set; }



        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchUsername_staff")]
        public int SearchUsername_Staff { get; set; }



        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchIdStatus")]
        public int SearchIdStatus { get; set; }
        public List<SelectListItem> ListStatus { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchOrderId")]
        public int SearchOrderId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchTrackingCode")]
        public string SearchTrackingCode { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchIssue")]
        public string SearchIssue { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchPriorityId")]
        public int SearchPriorityId { get; set; }
        public List<SelectListItem> ListPriority { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchStoreId")]
        public int SearchStoreId { get; set; }
        public List<SelectListItem> ListStores { get; set; }

        public List<SelectListItem> ListUsers { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchCategoryId")]
        public int SearchCategoryId { get; set; }
        public List<SelectListItem> ListcategoryTicket { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchTicketNumber")]
        public int SearchTicketNumber { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchOstanOrginId")]
        public int SearchOstanOrginId { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchOstanOrginId")]
        //public int SearchOstanDestinationId { get; set; }
        public List<SelectListItem> ListOstanTicket { get; set; }

        public DateTime? SearchTicketDateFrom { get; set; }
        public DateTime? SearchTicketDateTo { get; set; }


    }
}
