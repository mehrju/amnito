using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{
    public class Grid_ProirityTicket
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_Name")]
        public string Grid_ProirityTicket_Name { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_Sort")]
        public string Grid_ProirityTicket_Sort { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_IsActive")]
        public bool Grid_ProirityTicket_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_DateInsert")]
        public DateTime Grid_ProirityTicket_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_DateUpdate")]
        public DateTime? Grid_ProirityTicket_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_UserInsert")]
        public string Grid_ProirityTicket_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_UserUpdate")]
        public string Grid_ProirityTicket_UserUpdate { get; set; }
    }
}
