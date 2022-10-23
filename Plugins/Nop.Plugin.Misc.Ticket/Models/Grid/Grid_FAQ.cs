using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{

    public class Grid_FAQ //: BaseNopEntityModel
    {


        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQ_Question")]
        public string Grid_FAQ_Question { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQ_Answer")]
        public string Grid_FAQ_Answer { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQ_CategoryName")]
        public string Grid_FAQ_CategoryName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQ_IsActive")]
        public bool Grid_FAQ_IsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQ_DateInsert")]
        public DateTime Grid_FAQ_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQ_DateUpdate")]
        public DateTime? Grid_FAQ_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQ_UserInsert")]
        public string Grid_FAQ_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQ_UserUpdate")]
        public string Grid_FAQ_UserUpdate { get; set; }
    }
}
