using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{
    public class Grid_PatternAnswerTicket
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_Title")]
        public string Grid_PatternAnswerTicket_Title { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_Description")]
        public string Grid_PatternAnswerTicket_Description { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_IsActive")]
        public bool Grid_PatternAnswerTicket_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_DateInsert")]
        public DateTime Grid_PatternAnswerTicket_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_DateUpdate")]
        public DateTime? Grid_PatternAnswerTicket_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_UserInsert")]
        public string Grid_PatternAnswerTicket_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_UserUpdate")]
        public string Grid_PatternAnswerTicket_UserUpdate { get; set; }
    }
}
