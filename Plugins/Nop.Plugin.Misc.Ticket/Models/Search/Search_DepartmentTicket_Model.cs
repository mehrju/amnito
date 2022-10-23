using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_DepartmentTicket_Model
    {
        public Search_DepartmentTicket_Model()
        {
            ListStores = new List<SelectListItem>();
        }

        public bool Search_DepartmentTicket_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_DepartmentTicket_Name")]
        public string Search_DepartmentTicket_Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_DepartmentTicket_IsActive")]
        public bool Search_DepartmentTicket_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchStoreId")]
        public int Search_DepartmentTicket_StoreId { get; set; }
        public IList<SelectListItem> ListStores { get; set; }
    }
}
