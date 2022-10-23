using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{
    public class Grid_Ticket
    {
        public int Id { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_StoreName")]
        public string Grid_Ticket_StoreName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.DepartmentTicketName")]
        public string Grid_Ticket_Isuue { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_FullName")]
        public string Grid_Ticket_FullName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_DepName")]
        public string Grid_Ticket_DepName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_PriorityName")]
        public string Grid_Ticket_PriorityName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_OrderId")]
        public string Grid_Ticket_OrderId { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_TrackingCode")]
        public string Grid_Ticket_TrackingCode { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_Status")]
        public string Grid_Ticket_Status { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_DateInsert")]
        public DateTime Grid_Ticket_DateInsert { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_NameStaffOpen")]
        public string Grid_Ticket_NameStaffOpen { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_DateOpen")]
        public DateTime? Grid_Ticket_DateOpen { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_LastDateAnswer")]
        public DateTime? Grid_Ticket_LastDateAnswer { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_NameStaffLastAnswer")]
        public string Grid_Ticket_NameStaffLastAnswer { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_LastRank")]
        public int Grid_Ticket_LastRank { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_CategoryName")]
        public string Grid_Ticket_CategoryName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Ticket_TicketNumber")]
        public string Grid_Ticket_TicketNumber { get; set; }

        public string UrlPage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_IsActive")]
        public bool Grid_Damages_IsActive { get; set; }

        public bool TypeTicket { get; set; }
    }
}
