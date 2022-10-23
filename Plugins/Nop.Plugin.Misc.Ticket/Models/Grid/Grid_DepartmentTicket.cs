using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{
    public class Grid_DepartmentTicket
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_Name")]
        public string Grid_DepartmentTicket_Name { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_StoreName")]
        public string Grid_DepartmentTicket_StoreName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_IsActive")]
        public bool Grid_DepartmentTicket_IsActive { get; set; }
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
