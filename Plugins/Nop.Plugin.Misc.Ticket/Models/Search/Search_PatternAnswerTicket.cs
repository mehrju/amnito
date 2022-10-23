using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_PatternAnswerTicket
    {
        public bool Search_PatternAnswerTicket_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_PatternAnswerTicket_Title")]
        public string Search_PatternAnswerTicket_Title { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_PatternAnswerTicket_Descriptipn")]
        public string Search_PatternAnswerTicket_Descriptipn { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_PatternAnswerTicket_IsActive")]
        public bool Search_PatternAnswerTicket_IsActive { get; set; }
        
    }
}
