using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_CategoryTicket
    {
        public Search_CategoryTicket()
        {
            ListDepartment = new List<SelectListItem>();
        }

        public bool Search_CategoryTicket_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_CategoryTicket_NameCategory")]
        public string Search_CategoryTicket_NameCategory { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_CategoryTicket_IsActive")]
        public bool Search_CategoryTicket_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_CategoryTicket_DepartmentId")]
        public int Search_CategoryTicket_DepartmentId { get; set; }
        public IList<SelectListItem> ListDepartment { get; set; }
    }
}
