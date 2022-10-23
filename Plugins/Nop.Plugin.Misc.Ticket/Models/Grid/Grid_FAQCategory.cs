using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{
    public class Grid_FAQCategory
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_Name")]
        public string Grid_FAQCategory_Name { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_Sort")]
        public string Grid_FAQCategory_Sort { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_IsActive")]
        public bool Grid_FAQCategory_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_DateInsert")]
        public DateTime Grid_FAQCategory_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_DateUpdate")]
        public DateTime? Grid_FAQCategory_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_UserInsert")]
        public string Grid_FAQCategory_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_UserUpdate")]
        public string Grid_FAQCategory_UserUpdate { get; set; }
    }
}
