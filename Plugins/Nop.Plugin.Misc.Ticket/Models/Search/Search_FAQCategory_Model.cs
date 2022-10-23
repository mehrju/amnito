using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_FAQCategory_Model
    {
        

        public bool Search_FAQCategory_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_FAQCategory_Name")]
        public string Search_FAQCategory_Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_FAQCategory_IsActive")]
        public bool Search_FAQCategory_IsActive { get; set; }
        
    }
}
