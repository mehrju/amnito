using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Domain
{
    public class Tbl_FAQ : BaseEntity
    {
        public Tbl_FAQ()
        {
            ListCategory = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.FAQQuestion")]
        [DataType(DataType.Text)]
        public String Question { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.FAQAnswer")]
        [DataType(DataType.Text)]
        public String Answer { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.FAQIdCategory")]
        public int IdCategory { get; set; }
        public Tbl_FAQCategory FAQCategory { get; set; }



        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }

        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }

        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }

        [NotMapped]
        public IList<SelectListItem> ListCategory { get; set; }
    }
}
