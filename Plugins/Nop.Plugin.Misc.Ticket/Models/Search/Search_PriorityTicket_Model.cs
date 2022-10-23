using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_PriorityTicket_Model
    {
        

        public bool Search_PriorityTicket_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_PriorityTicket_Name")]
        public string Search_PriorityTicket_Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_PriorityTicket_IsActive")]
        public bool Search_PriorityTicket_IsActive { get; set; }
        
    }
}
