using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_FAQ_Model
    {
        public Search_FAQ_Model()
        {
            ListFAQCategory = new List<SelectListItem>();
        }

        public bool Search_FAQ_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_FAQ_Question")]
        public string Search_FAQ_Question { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_FAQ_Answer")]
        public string Search_FAQ_Answer { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_FAQ_IsActive")]
        public bool Search_FAQ_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.SearchFAQCategoryId")]
        public int SearchFAQCategoryId { get; set; }
        public List<SelectListItem> ListFAQCategory { get; set; }

    }
}
