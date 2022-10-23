using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{
    public class Grid_Damages
    {
        public int Id { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_StoreName")]
        public string Grid_Damages_StoreName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_FullName")]
        public string Grid_Damages_FullName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_TrackingCode")]
        public string Grid_Damages_TrackingCode { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_Status")]
        public string Grid_Damages_Status { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_DateInsert")]
        public DateTime Grid_Damages_DateInsert { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_NameStaff")]
        public string   Grid_Damages_NameStaff { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_DateOpen")]
        public DateTime? Grid_Damages_DateOpen { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_LastDateAnswer")]
        public DateTime? Grid_Damages_LastDateAnswer { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_NameStaffLastAnswer")]
        public string Grid_Damages_NameStaffLastAnswer { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_DamagesNumber")]
        public string Grid_Damages_DamagesNumber { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_NameGoods")]
        public string Grid_Damages_NameGoods { get; set; }

        public string UrlPage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_Damages_IsActive")]
        public bool Grid_Damages_IsActive { get; set; }
    }
}
