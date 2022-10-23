using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models
{
    public class Grid_RequestCOD
    {
        public int Id { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_StoreName")]
        public string Grid_RequestCOD_StoreName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_FullName")]
        public string Grid_RequestCOD_FullName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_Status")]
        public string Grid_RequestCOD_Status { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_DateInsert")]
        public String Grid_RequestCOD_DateInsert { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_NameStaff")]
        public string Grid_RequestCOD_NameStaff { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_DateOpen")]
        public string Grid_RequestCOD_DateOpen { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_LastDateAnswer")]
        public string Grid_RequestCOD_LastDateAnswer { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_NameStaffLastAnswer")]
        public string Grid_RequestCOD_NameStaffLastAnswer { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_RequestCODNumber")]
        public string Grid_RequestCOD_RequestCODNumber { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_RequestCODUserName")]
        public string Grid_RequestCOD_RequestCODUserName { get; set; }
    }

     public class RequestCodSearchResult
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Status { get; set; }
        public string DateInsert { get; set; }
        public int? StaffIdAccept { get; set; }
        public string DateStaffAccept { get; set; }
        public string DateStaffLastAnswer { get; set; }
        public int? StaffIdLastAnswer { get; set; }
        public string Username { get; set; }
    }
}
