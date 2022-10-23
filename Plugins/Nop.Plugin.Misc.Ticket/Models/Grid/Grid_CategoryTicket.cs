using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{
    public class Grid_CategoryTicket
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_DepartmentName")]
        public string Grid_CategoryTicket_DepartmentName { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_CategoryName")]
        public string Grid_CategoryTicket_CategoryName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_IsActive")]
        public bool Grid_CategoryTicket_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_DateInsert")]
        public DateTime Grid_CategoryTicket_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_DateUpdate")]
        public DateTime? Grid_CategoryTicket_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_UserInsert")]
        public string Grid_CategoryTicket_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_UserUpdate")]
        public string Grid_CategoryTicket_UserUpdate { get; set; }
    }
}
